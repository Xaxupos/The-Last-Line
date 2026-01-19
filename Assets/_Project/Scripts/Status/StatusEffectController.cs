using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class StatusEffectController : MonoBehaviour
{
    private sealed class StatusEffectInstance
    {
        public readonly int Id;
        public readonly string Key;
        public StatusEffectDefinition Definition;
        public Unit Source;
        public float Strength;
        public int Stacks;
        public float RemainingDuration;
        public float TotalDuration;
        public float NextTickTime;
        public readonly List<StatModifierHandle> ModifierHandles = new();

        public StatusEffectInstance(int id, string key, StatusEffectDefinition definition, Unit source)
        {
            Id = id;
            Key = key;
            Definition = definition;
            Source = source;
        }

        public StatusEffectInfo ToInfo()
        {
            return new StatusEffectInfo(Definition, RemainingDuration, TotalDuration, Stacks, Strength, Source);
        }
    }

    [SerializeField] private Unit owner;

    private readonly List<StatusEffectInstance> _instances = new();
    private readonly Dictionary<int, StatusEffectInstance> _instancesById = new();
    private readonly Dictionary<string, StatusEffectInstance> _instancesByKey = new();
    public SerializedDictionary<int, StatusEffectInfo> DebugById = new();
    public SerializedDictionary<string, StatusEffectInfo> DebugByKey = new();
    private int _nextId = 1;

    public event Action<StatusEffectInfo> OnEffectAdded;
    public event Action<StatusEffectInfo> OnEffectUpdated;
    public event Action<StatusEffectInfo> OnEffectRemoved;
    public event Action<StatusEffectInfo> OnEffectTicked;

    public StatusEffectHandle Apply(StatusEffectDefinition definition, Unit source, float strength = 1f, float durationOverride = -1f)
    {
        if (definition == null)
            return default;

        string key = definition.GetKey();
        float duration = durationOverride > 0f ? durationOverride : definition.durationSeconds;
        if (duration <= 0f)
            duration = float.PositiveInfinity;

        if (_instancesByKey.TryGetValue(key, out var existing))
        {
            float newStrength = (existing.Strength + strength) * 0.5f;
            existing.Strength = newStrength;
            existing.Stacks++;
            existing.Source = source;

            if (float.IsPositiveInfinity(existing.RemainingDuration) || float.IsPositiveInfinity(duration))
                existing.RemainingDuration = float.PositiveInfinity;
            else
                existing.RemainingDuration += duration;

            existing.TotalDuration = existing.RemainingDuration;

            ReapplyModifiers(existing);
            OnEffectUpdated?.Invoke(existing.ToInfo());
            return new StatusEffectHandle(existing.Id);
        }

        var instance = new StatusEffectInstance(_nextId++, key, definition, source)
        {
            Strength = strength,
            Stacks = 1,
            RemainingDuration = duration,
            TotalDuration = duration,
            NextTickTime = ShouldTick(definition) ? Time.time + definition.tickIntervalSeconds : -1f
        };

        ApplyModifiers(instance);
        _instances.Add(instance);
        _instancesById[instance.Id] = instance;
        _instancesByKey[key] = instance;

        OnEffectAdded?.Invoke(instance.ToInfo());
        SyncDebug();
        return new StatusEffectHandle(instance.Id);
    }

    public bool Remove(StatusEffectHandle handle)
    {
        if (!_instancesById.TryGetValue(handle.Id, out var instance))
            return false;

        RemoveInstance(instance);
        return true;
    }

    public bool Remove(StatusEffectDefinition definition)
    {
        if (definition == null)
            return false;

        if (!_instancesByKey.TryGetValue(definition.GetKey(), out var instance))
            return false;

        RemoveInstance(instance);
        return true;
    }

    public int RemoveFromSource(Unit source)
    {
        if (source == null)
            return 0;

        int removed = 0;
        for (int i = _instances.Count - 1; i >= 0; i--)
        {
            if (_instances[i].Source != source)
                continue;

            RemoveInstance(_instances[i]);
            removed++;
        }

        return removed;
    }

    public int RemoveAllDispelled()
    {
        int removed = 0;
        for (int i = _instances.Count - 1; i >= 0; i--)
        {
            if (!_instances[i].Definition.canBeDispelled)
                continue;

            RemoveInstance(_instances[i]);
            removed++;
        }

        return removed;
    }

    public void ClearAll()
    {
        for (int i = _instances.Count - 1; i >= 0; i--)
            RemoveInstance(_instances[i]);
    }

    public void GetActiveEffects(List<StatusEffectInfo> buffer)
    {
        buffer.Clear();
        for (int i = 0; i < _instances.Count; i++)
            buffer.Add(_instances[i].ToInfo());
    }

    private void Update()
    {
        if (owner.IsDead)
        {
            ClearAll();
            return;
        }

        if (_instances.Count == 0)
            return;

        float now = Time.time;
        for (int i = _instances.Count - 1; i >= 0; i--)
        {
            var instance = _instances[i];
            var def = instance.Definition;

            if (def.removeOnSourceDeath && instance.Source != null && instance.Source.IsDead)
            {
                RemoveInstance(instance);
                continue;
            }

            if (!float.IsPositiveInfinity(instance.RemainingDuration))
            {
                instance.RemainingDuration -= Time.deltaTime;
                if (instance.RemainingDuration <= 0f)
                {
                    RemoveInstance(instance);
                    continue;
                }
            }

            if (ShouldTick(def) && now >= instance.NextTickTime)
            {
                Tick(instance);
                instance.NextTickTime = now + def.tickIntervalSeconds;
            }
        }

        SyncDebug();
    }

    private void ApplyModifiers(StatusEffectInstance instance)
    {
        if (owner.statsComponent == null || instance.Definition.statModifiers == null)
            return;

        foreach (var entry in instance.Definition.statModifiers)
        {
            float value = entry.value * instance.Strength;
            var handle = owner.statsComponent.AddModifier(entry.id, new StatModifier(entry.type, value));
            instance.ModifierHandles.Add(handle);
        }
    }

    private void ReapplyModifiers(StatusEffectInstance instance)
    {
        if (owner.statsComponent == null)
            return;

        for (int i = 0; i < instance.ModifierHandles.Count; i++)
            owner.statsComponent.RemoveModifier(instance.ModifierHandles[i]);

        instance.ModifierHandles.Clear();
        ApplyModifiers(instance);
    }

    private void RemoveInstance(StatusEffectInstance instance)
    {
        for (int i = 0; i < instance.ModifierHandles.Count; i++)
            owner.statsComponent.RemoveModifier(instance.ModifierHandles[i]);

        _instances.Remove(instance);
        _instancesById.Remove(instance.Id);
        _instancesByKey.Remove(instance.Key);
        SyncDebug();
        OnEffectRemoved?.Invoke(instance.ToInfo());
    }

    private void Tick(StatusEffectInstance instance)
    {
        if (owner.health == null)
            return;

        float value = instance.Definition.tickValue * instance.Strength;
        if (value <= 0f)
            return;

        if (string.Equals(instance.Definition.GetKey(), "BLEED", StringComparison.OrdinalIgnoreCase))
            Debug.Log($"BLEED TICK on {owner.name}: {value:0.##}", owner);

        if (instance.Definition.tickIsHeal)
            owner.health.Heal(value);
        else
            owner.health.TakeDamage(value);

        PlayTickFeedback(instance);
        OnEffectTicked?.Invoke(instance.ToInfo());
    }

    private void PlayTickFeedback(StatusEffectInstance instance)
    {
        var def = instance.Definition;
        if (def.tickVfxPrefab != null)
            Instantiate(def.tickVfxPrefab, owner.transform.position, Quaternion.identity);

        if (def.tickSfx != null)
            AudioSource.PlayClipAtPoint(def.tickSfx, owner.transform.position);
    }

    private static bool ShouldTick(StatusEffectDefinition def)
    {
        return def.tickIntervalSeconds > 0f && def.tickValue > 0f;
    }

    private void SyncDebug()
    {
        if (DebugById == null)
            DebugById = new SerializedDictionary<int, StatusEffectInfo>();
        if (DebugByKey == null)
            DebugByKey = new SerializedDictionary<string, StatusEffectInfo>();

        DebugById.Clear();
        DebugByKey.Clear();

        for (int i = 0; i < _instances.Count; i++)
        {
            var info = _instances[i].ToInfo();
            DebugById[_instances[i].Id] = info;
            DebugByKey[_instances[i].Key] = info;
        }
    }
}
