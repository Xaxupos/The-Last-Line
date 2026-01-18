using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Effects/Loot Modifier")]
public class LootModifierUpgradeEffect : UpgradeEffectDefinition
{
    [SerializeField] private bool applyToAllCurrencies = true;
    [SerializeField] private CurrencyType[] currencyFilter;
    [Range(0f, 100f)] [SerializeField] private float chancePercent = 100f;
    [SerializeField] private float multiplierPerStack = 1f;
    [SerializeField] private int flatBonusPerStack;

    public override int ModifyLoot(CurrencyType type, int amount, int stacks)
    {
        if (amount <= 0)
            return amount;

        if (!Matches(type))
            return amount;

        int stackCount = Mathf.Max(1, stacks);
        float finalChance = Mathf.Clamp(chancePercent * stackCount, 0f, 100f);
        if (finalChance <= 0f)
            return amount;

        if (!Roll(finalChance))
            return amount;

        float multiplier = 1f + (multiplierPerStack - 1f) * stackCount;
        int result = Mathf.RoundToInt(amount * multiplier) + flatBonusPerStack * stackCount;
        return Mathf.Max(0, result);
    }

    private bool Matches(CurrencyType type)
    {
        if (applyToAllCurrencies || currencyFilter == null || currencyFilter.Length == 0)
            return true;

        for (int i = 0; i < currencyFilter.Length; i++)
            if (currencyFilter[i] == type)
                return true;

        return false;
    }

    private static bool Roll(float chancePercent)
    {
        return Random.value * 100f <= chancePercent;
    }
}
