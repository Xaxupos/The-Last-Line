using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Effects/Round Modifier")]
public class RoundModifierUpgradeEffect : UpgradeEffectDefinition
{
    [SerializeField] private float spawnRateAddPerStack;
    [SerializeField] private float spawnRateMultiplierPerStack = 1f;
    [SerializeField] private float timeLimitAddPerStack;
    [SerializeField] private float timeLimitMultiplierPerStack = 1f;

    public override void ModifyRound(ref float spawnRate, ref float timeLimitSeconds, int stacks)
    {
        int stackCount = Mathf.Max(1, stacks);

        spawnRate += spawnRateAddPerStack * stackCount;
        spawnRate *= 1f + (spawnRateMultiplierPerStack - 1f) * stackCount;
        spawnRate = Mathf.Max(0f, spawnRate);

        timeLimitSeconds += timeLimitAddPerStack * stackCount;
        timeLimitSeconds *= 1f + (timeLimitMultiplierPerStack - 1f) * stackCount;
        timeLimitSeconds = Mathf.Max(0f, timeLimitSeconds);
    }
}
