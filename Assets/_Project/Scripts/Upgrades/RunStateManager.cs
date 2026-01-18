using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunStateManager : MonoBehaviourSingleton<RunStateManager>
{
    [Serializable]
    public class UpgradeStack
    {
        public UpgradeDefinition definition;
        public int stacks = 1;
    }

    [SerializeField] private List<UpgradeStack> upgrades = new();

    private UnitsManager _unitsManager;

    public IReadOnlyList<UpgradeStack> Upgrades => upgrades;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        TryBindUnitsManager();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        UnbindUnitsManager();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryBindUnitsManager();
    }

    private void TryBindUnitsManager()
    {
        if (!UnitsManager.HasInstance)
        {
            UnbindUnitsManager();
            return;
        }

        var manager = UnitsManager.Instance;
        if (manager == _unitsManager)
            return;

        UnbindUnitsManager();
        _unitsManager = manager;

        _unitsManager.OnUnitRegistered += HandleUnitRegistered;
        ApplyAllUpgradesToUnits(_unitsManager.Players);
        ApplyAllUpgradesToUnits(_unitsManager.Enemies);
    }

    private void UnbindUnitsManager()
    {
        if (_unitsManager == null)
            return;

        _unitsManager.OnUnitRegistered -= HandleUnitRegistered;
        _unitsManager = null;
    }

    private void HandleUnitRegistered(Unit unit)
    {
        ApplyToUnit(unit);
    }

    public void AddUpgrade(UpgradeDefinition definition, int stacks = 1)
    {
        if (definition == null || stacks <= 0)
            return;

        var entry = GetEntry(definition);
        int previousStacks = entry != null ? entry.stacks : 0;
        int maxStacks = definition.maxStacks > 0 ? definition.maxStacks : int.MaxValue;
        int nextStacks = Mathf.Min(previousStacks + stacks, maxStacks);
        if (nextStacks == previousStacks)
            return;

        if (entry == null)
        {
            entry = new UpgradeStack
            {
                definition = definition,
                stacks = nextStacks
            };
            upgrades.Add(entry);
        }
        else
        {
            entry.stacks = nextStacks;
        }

        ApplyUpgradeToAllUnits(definition, nextStacks);
    }

    public bool RemoveUpgrade(UpgradeDefinition definition, int stacks = 1)
    {
        if (definition == null || stacks <= 0)
            return false;

        var entry = GetEntry(definition);
        if (entry == null)
            return false;

        int nextStacks = entry.stacks - stacks;
        if (nextStacks <= 0)
        {
            upgrades.Remove(entry);
            RemoveUpgradeFromAllUnits(definition);
            return true;
        }

        entry.stacks = nextStacks;
        ApplyUpgradeToAllUnits(definition, nextStacks);
        return true;
    }

    public int GetStacks(UpgradeDefinition definition)
    {
        var entry = GetEntry(definition);
        return entry != null ? entry.stacks : 0;
    }

    public void ClearAllUpgrades()
    {
        for (int i = 0; i < upgrades.Count; i++)
            RemoveUpgradeFromAllUnits(upgrades[i].definition);

        upgrades.Clear();
    }

    public void ApplyToUnit(Unit unit)
    {
        if (unit == null || unit.IsDead)
            return;

        for (int i = 0; i < upgrades.Count; i++)
        {
            var entry = upgrades[i];
            if (entry.definition == null || entry.stacks <= 0)
                continue;

            ApplyUpgradeToUnit(unit, entry.definition, entry.stacks);
        }
    }

    public void ApplyToRound(RoundRuntimeState runtime)
    {
        if (runtime == null)
            return;

        float spawnRate = runtime.BaseDefinition != null ? runtime.BaseDefinition.baseSpawnRate : runtime.SpawnRate;
        float timeLimit = runtime.BaseDefinition != null ? runtime.BaseDefinition.timeLimitSeconds : runtime.TimeLimitSeconds;

        for (int i = 0; i < upgrades.Count; i++)
        {
            var entry = upgrades[i];
            if (entry.definition == null || entry.stacks <= 0)
                continue;

            var effects = entry.definition.effects;
            if (effects == null)
                continue;

            for (int e = 0; e < effects.Count; e++)
                effects[e]?.ModifyRound(ref spawnRate, ref timeLimit, entry.stacks);
        }

        runtime.SetSpawnRate(spawnRate);
        runtime.SetTimeLimit(timeLimit);
    }

    public int ModifyLootAmount(CurrencyType type, int amount)
    {
        int result = amount;
        if (result <= 0)
            return result;

        for (int i = 0; i < upgrades.Count; i++)
        {
            var entry = upgrades[i];
            if (entry.definition == null || entry.stacks <= 0)
                continue;

            var effects = entry.definition.effects;
            if (effects == null)
                continue;

            for (int e = 0; e < effects.Count; e++)
            {
                if (effects[e] == null)
                    continue;

                result = effects[e].ModifyLoot(type, result, entry.stacks);
                if (result <= 0)
                    return 0;
            }
        }

        return Mathf.Max(0, result);
    }

    private UpgradeStack GetEntry(UpgradeDefinition definition)
    {
        if (definition == null)
            return null;

        for (int i = 0; i < upgrades.Count; i++)
        {
            var entry = upgrades[i];
            if (entry.definition == definition)
                return entry;
        }

        return null;
    }

    private void ApplyAllUpgradesToUnits(IReadOnlyList<Unit> units)
    {
        if (units == null)
            return;

        for (int i = 0; i < units.Count; i++)
            ApplyToUnit(units[i]);
    }

    private void ApplyUpgradeToAllUnits(UpgradeDefinition definition, int stacks)
    {
        if (_unitsManager == null)
            return;

        ApplyUpgradeToUnits(_unitsManager.Players, definition, stacks);
        ApplyUpgradeToUnits(_unitsManager.Enemies, definition, stacks);
    }

    private void RemoveUpgradeFromAllUnits(UpgradeDefinition definition)
    {
        if (_unitsManager == null)
            return;

        RemoveUpgradeFromUnits(_unitsManager.Players, definition);
        RemoveUpgradeFromUnits(_unitsManager.Enemies, definition);
    }

    private void ApplyUpgradeToUnits(IReadOnlyList<Unit> units, UpgradeDefinition definition, int stacks)
    {
        if (units == null)
            return;

        for (int i = 0; i < units.Count; i++)
            ApplyUpgradeToUnit(units[i], definition, stacks);
    }

    private void RemoveUpgradeFromUnits(IReadOnlyList<Unit> units, UpgradeDefinition definition)
    {
        if (units == null)
            return;

        for (int i = 0; i < units.Count; i++)
            RemoveUpgradeFromUnit(units[i], definition);
    }

    private void ApplyUpgradeToUnit(Unit unit, UpgradeDefinition definition, int stacks)
    {
        if (unit == null || unit.IsDead || definition == null)
            return;

        RemoveUpgradeFromUnit(unit, definition);

        var effects = definition.effects;
        if (effects == null)
            return;

        for (int i = 0; i < effects.Count; i++)
            effects[i]?.ApplyToUnit(unit, stacks, definition);
    }

    private static void RemoveUpgradeFromUnit(Unit unit, UpgradeDefinition definition)
    {
        if (unit == null || definition == null)
            return;

        unit.statsComponent?.RemoveModifiersFromSource(definition);
        unit.onHitEffects?.RemoveRuntimeEffectsFromSource(definition);
        unit.attackController?.RemoveRuntimeOnHitActionsFromSource(definition);
    }
}
