using System;
using System.Collections.Generic;
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

    private struct RuntimeEffect
    {
        public OnHitEffectEntry Entry;
        public object Source;
    }

    [SerializeField] private Unit owner;
    [SerializeField] private List<OnHitEffectEntry> effects = new();
    private readonly List<RuntimeEffect> _runtimeEffects = new();

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
        if (target == null || target.statusEffects == null)
            return;

        for (int i = 0; i < effects.Count; i++)
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

        for (int i = 0; i < _runtimeEffects.Count; i++)
        {
            var entry = _runtimeEffects[i].Entry;
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

    public void AddRuntimeEffect(StatusEffectDefinition effect, float chancePercent, float strength, object source)
    {
        if (effect == null)
            return;

        var entry = new OnHitEffectEntry
        {
            effect = effect,
            chancePercent = chancePercent,
            strength = strength
        };

        _runtimeEffects.Add(new RuntimeEffect
        {
            Entry = entry,
            Source = source
        });
    }

    public void RemoveRuntimeEffectsFromSource(object source)
    {
        for (int i = _runtimeEffects.Count - 1; i >= 0; i--)
        {
            if (!ReferenceEquals(_runtimeEffects[i].Source, source))
                continue;

            _runtimeEffects.RemoveAt(i);
        }
    }

    private static bool Roll(float chancePercent)
    {
        return UnityEngine.Random.value * 100f <= chancePercent;
    }
}
