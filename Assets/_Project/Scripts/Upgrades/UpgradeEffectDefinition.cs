using UnityEngine;

public abstract class UpgradeEffectDefinition : ScriptableObject
{
    public virtual void ApplyToUnit(Unit unit, int stacks, object source) { }
    public virtual void ModifyRound(ref float spawnRate, ref float timeLimitSeconds, int stacks) { }
    public virtual int ModifyLoot(CurrencyType type, int amount, int stacks) { return amount; }
}
