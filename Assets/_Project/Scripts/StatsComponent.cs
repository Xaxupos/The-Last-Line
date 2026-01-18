using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsComponent : MonoBehaviour
{
    private sealed class StatModifierInstance
    {
        public readonly int Id;
        public readonly StatModifier Modifier;
        public readonly object Source;
        public readonly float ExpiresAt;

        public bool IsTimed => ExpiresAt >= 0f;

        public StatModifierInstance(int id, StatModifier modifier, object source, float expiresAt)
        {
            Id = id;
            Modifier = modifier;
            Source = source;
            ExpiresAt = expiresAt;
        }
    }

    private readonly Dictionary<StatId, float> _baseStats = new();
    private readonly Dictionary<StatId, List<StatModifierInstance>> _modifiers = new();
    private int _nextModifierId = 1;
    private bool _hasTimedModifiers;

    public event Action<StatId> OnStatChanged;

    public float GetBase(StatId id)
    {
        return _baseStats.TryGetValue(id, out var baseValue) ? baseValue : 0f;
    }

    public void SetBase(StatId id, float value)
    {
        _baseStats[id] = value;
        OnStatChanged?.Invoke(id);
    }

    public void AddBase(StatId id, float delta)
    {
        SetBase(id, GetBase(id) + delta);
    }

    public void SetBaseStats(IEnumerable<StatBaseEntry> entries, bool clearExisting = true)
    {
        if (entries == null)
            return;

        if (clearExisting)
            _baseStats.Clear();

        foreach (var entry in entries)
        {
            _baseStats[entry.id] = entry.value;
            OnStatChanged?.Invoke(entry.id);
        }
    }

    public StatModifierHandle AddModifier(StatId id, StatModifier mod, float durationSeconds = -1f, object source = null)
    {
        if (!_modifiers.TryGetValue(id, out var list))
        {
            list = new List<StatModifierInstance>();
            _modifiers[id] = list;
        }

        float expiresAt = durationSeconds > 0f ? Time.time + durationSeconds : -1f;
        var instance = new StatModifierInstance(_nextModifierId++, mod, source, expiresAt);
        list.Add(instance);

        if (instance.IsTimed)
            _hasTimedModifiers = true;

        OnStatChanged?.Invoke(id);
        return new StatModifierHandle(id, instance.Id);
    }

    public bool RemoveModifier(StatModifierHandle handle)
    {
        if (!_modifiers.TryGetValue(handle.StatId, out var list))
            return false;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i].Id != handle.Id)
                continue;

            list.RemoveAt(i);
            OnStatChanged?.Invoke(handle.StatId);
            return true;
        }

        return false;
    }

    public int RemoveModifiersFromSource(object source)
    {
        if (source == null)
            return 0;

        int removed = 0;
        foreach (var kvp in _modifiers)
        {
            var list = kvp.Value;
            bool changed = false;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!ReferenceEquals(list[i].Source, source))
                    continue;

                list.RemoveAt(i);
                removed++;
                changed = true;
            }

            if (changed)
                OnStatChanged?.Invoke(kvp.Key);
        }

        return removed;
    }

    public void ClearModifiers()
    {
        foreach (var kvp in _modifiers)
            if (kvp.Value.Count > 0)
                OnStatChanged?.Invoke(kvp.Key);

        _modifiers.Clear();
        _hasTimedModifiers = false;
    }

    public float GetFinal(StatId id)
    {
        if (!_baseStats.TryGetValue(id, out var baseValue))
            return 0f;

        float flat = 0f;
        float percent = 0f;
        float mult = 1f;

        if (_modifiers.TryGetValue(id, out var mods))
        {
            foreach (var m in mods)
            {
                switch (m.Modifier.Type)
                {
                    case StatModType.Flat: flat += m.Modifier.Value; break;
                    case StatModType.Percent: percent += m.Modifier.Value; break;
                    case StatModType.Multiplier: mult *= m.Modifier.Value; break;
                }
            }
        }

        return (baseValue + flat) * (1f + percent) * mult;
    }

    public float Get(StatId id)
    {
        return GetFinal(id);
    }

    private void Update()
    {
        if (!_hasTimedModifiers)
            return;

        float now = Time.time;
        bool anyTimed = false;

        foreach (var kvp in _modifiers)
        {
            var list = kvp.Value;
            bool changed = false;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var instance = list[i];
                if (instance.IsTimed && now >= instance.ExpiresAt)
                {
                    list.RemoveAt(i);
                    changed = true;
                    continue;
                }

                if (instance.IsTimed)
                    anyTimed = true;
            }

            if (changed)
                OnStatChanged?.Invoke(kvp.Key);
        }

        _hasTimedModifiers = anyTimed;
    }
}
