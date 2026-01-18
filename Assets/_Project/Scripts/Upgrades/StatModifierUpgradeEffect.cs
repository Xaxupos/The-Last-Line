using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Effects/Stat Modifier")]
public class StatModifierUpgradeEffect : UpgradeEffectDefinition
{
    [SerializeField] private UnitTeamFilter teamFilter = UnitTeamFilter.Player;
    [SerializeField] private UnitDefinition[] unitFilter;
    [SerializeField] private StatModifierEntry[] modifiers;

    public override void ApplyToUnit(Unit unit, int stacks, object source)
    {
        if (!Matches(unit))
            return;

        if (unit.statsComponent == null || modifiers == null)
            return;

        float multiplier = Mathf.Max(1, stacks);
        for (int i = 0; i < modifiers.Length; i++)
        {
            var entry = modifiers[i];
            float value = entry.value * multiplier;
            unit.statsComponent.AddModifier(entry.id, new StatModifier(entry.type, value), source: source);
        }
    }

    private bool Matches(Unit unit)
    {
        if (unit == null)
            return false;

        if (teamFilter == UnitTeamFilter.Player && unit.team != Team.Player)
            return false;
        if (teamFilter == UnitTeamFilter.Enemy && unit.team != Team.Enemy)
            return false;

        if (unitFilter != null && unitFilter.Length > 0)
        {
            for (int i = 0; i < unitFilter.Length; i++)
                if (unit.definition == unitFilter[i])
                    return true;
            return false;
        }

        return true;
    }
}
