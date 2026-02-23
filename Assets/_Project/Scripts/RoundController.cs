using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class RoundController : MonoBehaviour
{
    [SerializeField] private EnemySpawnerController enemySpawner;
    [SerializeField] private PlayerUnitsSpawner playerUnitsSpawner;
    [SerializeField] private RoundRuntimeState runtimeState;
    [Header("Events")]
    [SerializeField] private UnityEvent OnRoundStarted;
    [SerializeField] private UnityEvent OnRoundEnded;

    private bool _running;
    private float _startTime;

    private void Start()
    {
        if (RunStateManager.HasInstance && RunStateManager.Instance.TryConsumePendingRound(out var round))
            StartRound(round);
    }

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
        if (playerUnitsSpawner == null)
            playerUnitsSpawner = FindFirstObjectByType<PlayerUnitsSpawner>();

        if (playerUnitsSpawner != null)
        {
            playerUnitsSpawner.SpawnFromDeck();
        }
        else
        {
            var deck = PlayerDeck.InstanceOrNull;
            if (deck != null && deck.HasAnyUnits())
                Debug.LogWarning("RoundController could not find PlayerUnitsSpawner. Player units will not spawn.", this);
        }

        enemySpawner.StartSpawning(runtimeState);
        OnRoundStarted?.Invoke();
    }

    private void Update()
    {
        if (!_running || runtimeState == null || !runtimeState.IsRunning)
            return;

        runtimeState.Tick(Time.deltaTime);
        if (runtimeState.IsTimeUp())
            EndRound();
    }

    private void OnUnitDied(Unit _)
    {
        if (!_running) return;

        if (UnitsManager.Instance.AlivePlayers <= 0)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        if (!_running)
            return;

        _running = false;

        if (enemySpawner != null)
            enemySpawner.StopSpawning();

        if (runtimeState != null)
            runtimeState.Stop();

        Debug.Log($"ROUND ENDED in {Time.time - _startTime:0.00}s");

        OnRoundEnded?.Invoke();
    }
}
