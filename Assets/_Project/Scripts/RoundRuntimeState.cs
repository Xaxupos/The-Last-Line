using UnityEngine;

public class RoundRuntimeState : MonoBehaviour
{
    [SerializeField] private RoundDefinition baseDefinition;
    [SerializeField] private float spawnRate;
    [SerializeField] private float timeLimitSeconds;
    [SerializeField] private float timeLeft;
    [SerializeField] private float elapsedSeconds;
    [SerializeField] private bool running;

    public RoundDefinition BaseDefinition => baseDefinition;
    public float SpawnRate => spawnRate;
    public float TimeLimitSeconds => timeLimitSeconds;
    public float TimeLeft => timeLeft;
    public float ElapsedSeconds => elapsedSeconds;
    public bool IsRunning => running;

    public void Initialize(RoundDefinition definition)
    {
        baseDefinition = definition;
        spawnRate = definition != null ? definition.baseSpawnRate : 0f;
        timeLimitSeconds = definition != null ? definition.timeLimitSeconds : 0f;
        timeLeft = timeLimitSeconds;
        elapsedSeconds = 0f;
        running = true;
    }

    public void Stop()
    {
        running = false;
    }

    public void SetSpawnRate(float value)
    {
        spawnRate = Mathf.Max(0f, value);
    }

    public void SetTimeLimit(float value)
    {
        timeLimitSeconds = Mathf.Max(0f, value);
        if (timeLimitSeconds <= 0f)
        {
            timeLeft = 0f;
            return;
        }

        if (elapsedSeconds <= 0f)
        {
            timeLeft = timeLimitSeconds;
            return;
        }

        timeLeft = Mathf.Max(0f, timeLimitSeconds - elapsedSeconds);
    }

    public void Tick(float deltaTime)
    {
        if (!running)
            return;

        elapsedSeconds += deltaTime;

        if (timeLimitSeconds <= 0f)
            return;

        timeLeft = Mathf.Max(0f, timeLeft - deltaTime);
    }

    public bool IsTimeUp()
    {
        return timeLimitSeconds > 0f && timeLeft <= 0f;
    }
}
