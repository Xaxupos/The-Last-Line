using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Effects/On Hit Status")]
public class OnHitStatusUpgradeEffect : UpgradeEffectDefinition
{
    [SerializeField] private UnitTeamFilter teamFilter = UnitTeamFilter.Player;
    [SerializeField] private UnitDefinition[] unitFilter;
    [SerializeField] private StatusEffectDefinition statusEffect;
    [Range(0f, 100f)] [SerializeField] private float chancePercent = 100f;
    [SerializeField] private float strength = 1f;

    public override void ApplyToUnit(Unit unit, int stacks, object source)
    {
        if (!Matches(unit))
            return;

        if (unit.onHitEffects == null || statusEffect == null)
            return;

        float finalChance = chancePercent * Mathf.Max(1, stacks);
        float finalStrength = strength * Mathf.Max(1, stacks);
        unit.onHitEffects.AddRuntimeEffect(statusEffect, finalChance, finalStrength, source);
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
