using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Effects/On Hit Action")]
public class OnHitActionUpgradeEffect : UpgradeEffectDefinition
{
    [SerializeField] private UnitTeamFilter teamFilter = UnitTeamFilter.Player;
    [SerializeField] private UnitDefinition[] unitFilter;
    [SerializeField] private OnHitActionDefinition onHitAction;

    public override void ApplyToUnit(Unit unit, int stacks, object source)
    {
        if (!Matches(unit))
            return;

        if (unit.attackController == null || onHitAction == null)
            return;

        for (int i = 0; i < Mathf.Max(1, stacks); i++)
            unit.attackController.AddRuntimeOnHitAction(onHitAction, source);
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
