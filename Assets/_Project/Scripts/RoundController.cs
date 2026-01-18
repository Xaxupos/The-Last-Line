using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class RoundController : MonoBehaviour
{
    [SerializeField] private EnemySpawnerController enemySpawner;
    [SerializeField] private RoundRuntimeState runtimeState;
    [Header("Events")]
    [SerializeField] private UnityEvent OnRoundStarted;
    [SerializeField] private UnityEvent OnRoundEnded;
    [SerializeField] private UnityEvent OnRoundWon;
    [SerializeField] private UnityEvent OnRoundLost;

    private bool _running;
    private float _startTime;

    private void OnEnable()
    {
        UnitsManager.Instance.OnUnitDied += OnUnitDied;
    }

    private void OnDisable()
    {
        if (UnitsManager.HasInstance)
            UnitsManager.Instance.OnUnitDied -= OnUnitDied;
    }

    [Button]
    public void StartRound(RoundDefinition round)
    {
        if (round == null)
        {
            Debug.LogError("RoundController.StartRound called with null round.", this);
            return;
        }

        if (enemySpawner == null)
        {
            Debug.LogError("RoundController missing EnemySpawnerController reference.", this);
            return;
        }

        if (runtimeState == null)
        {
            Debug.LogError("RoundController missing RoundRuntimeState reference.", this);
            return;
        }

        runtimeState.Initialize(round);
        if (RunStateManager.HasInstance)
            RunStateManager.Instance.ApplyToRound(runtimeState);
        _running = true;
        _startTime = Time.time;
        enemySpawner.StartSpawning(runtimeState);
        OnRoundStarted?.Invoke();
    }

    private void Update()
    {
        if (!_running || runtimeState == null || !runtimeState.IsRunning)
            return;

        runtimeState.Tick(Time.deltaTime);
        if (runtimeState.IsTimeUp())
            EndRound(true);
    }

    private void OnUnitDied(Unit _)
    {
        if (!_running) return;

        if (UnitsManager.Instance.AlivePlayers <= 0)
        {
            EndRound(false);
        }
    }

    private void EndRound(bool won)
    {
        if (!_running)
            return;

        _running = false;

        if (enemySpawner != null)
            enemySpawner.StopSpawning();

        if (runtimeState != null)
            runtimeState.Stop();

        if (won)
        {
            Debug.Log($"WIN in {Time.time - _startTime:0.00}s");
            OnRoundWon?.Invoke();
        }
        else
        {
            Debug.Log("LOSE");
            OnRoundLost?.Invoke();
        }

        OnRoundEnded?.Invoke();
    }
}
