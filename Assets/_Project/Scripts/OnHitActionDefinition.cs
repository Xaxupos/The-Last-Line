using UnityEngine;

public abstract class OnHitActionDefinition : ScriptableObject
{
    public abstract void Apply(Unit attacker, Unit target, float baseDamage);
}
