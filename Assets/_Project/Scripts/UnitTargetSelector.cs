using System.Collections.Generic;
using UnityEngine;

public abstract class UnitTargetSelector : ScriptableObject
{
    [SerializeField] private TargetTeam targetTeam = TargetTeam.Enemy;
    [SerializeField] private bool includeSelf;

    public TargetTeam TargetTeam => targetTeam;
    public bool IncludeSelf => includeSelf;

    public abstract Unit SelectTarget(Unit self, IReadOnlyList<Unit> candidates);

    protected bool IsCandidateValid(Unit self, Unit candidate)
    {
        if (candidate == null || !candidate.isActiveAndEnabled)
            return false;

        if (candidate.IsDead)
            return false;

        if (!includeSelf && candidate == self)
            return false;

        return true;
    }
}
