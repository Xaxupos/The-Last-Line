using UnityEngine;
using UnityEngine.Events;

public class RoundController : MonoBehaviour
{
    [SerializeField] private EnemySpawnerController enemySpawner;
    [Header("Events")]
    [SerializeField] private UnityEvent OnRoundStarted;
    [SerializeField] private UnityEvent OnRoundEnded;
    [SerializeField] private UnityEvent OnRoundWon;
    [SerializeField] private UnityEvent OnRoundLost;

    private bool _running;
    private float _startTime;
    private RoundDefinition _currentRound;

    private void OnEnable()
    {
        UnitsManager.Instance.OnUnitDied += OnUnitDied;
    }

    private void OnDisable()
    {
        if (UnitsManager.HasInstance)
            UnitsManager.Instance.OnUnitDied -= OnUnitDied;
    }

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

        _currentRound = round;
        _running = true;
        _startTime = Time.time;
        enemySpawner.StartSpawning(round);
        OnRoundStarted?.Invoke();
    }

    private void OnUnitDied(Unit _)
    {
        if (!_running) return;

        if (UnitsManager.Instance.AliveEnemies <= 0)
        {
            EndRound(true);
        }
        else if (UnitsManager.Instance.AlivePlayers <= 0)
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
        _currentRound = null;
    }
}
