using System;
using UnityEngine;

public class UnitOnHitEffectController : MonoBehaviour
{
    [Serializable]
    public struct OnHitEffectEntry
    {
        public StatusEffectDefinition effect;
        [Range(0f, 100f)] public float chancePercent;
        public float strength;
    }

    [SerializeField] private Unit owner;
    [SerializeField] private OnHitEffectEntry[] effects;

    private void Awake()
    {
        if (owner.attackController == null)
        {
            Debug.LogError("UnitOnHitEffectController missing UnitAttackController reference.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!enabled)
            return;

        owner.attackController.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        if (owner != null && owner.attackController != null)
            owner.attackController.OnHit -= HandleHit;
    }

    private void HandleHit(Unit attacker, Unit target, float damage)
    {
        if (target == null || target.statusEffects == null || effects == null)
            return;

        for (int i = 0; i < effects.Length; i++)
        {
            var entry = effects[i];
            if (entry.effect == null)
                continue;

            if (entry.chancePercent <= 0f)
                continue;

            if (!Roll(entry.chancePercent))
                continue;

            float strength = entry.strength <= 0f ? 1f : entry.strength;
            target.statusEffects.Apply(entry.effect, attacker, strength);
        }
    }

    private static bool Roll(float chancePercent)
    {
        return UnityEngine.Random.value * 100f <= chancePercent;
    }
}
