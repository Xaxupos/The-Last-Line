using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Target Selectors/Lowest Health")]
public class LowestHealthUnitSelector : UnitTargetSelector
{
    [SerializeField] private bool usePercent = true;

    public override Unit SelectTarget(Unit self, IReadOnlyList<Unit> candidates)
    {
        if (candidates == null)
            return null;

        Unit best = null;
        float bestValue = float.MaxValue;

        for (int i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates[i];
            if (!IsCandidateValid(self, candidate) || candidate.health == null)
                continue;

            float max = Mathf.Max(1f, candidate.health.Max);
            float value = usePercent ? candidate.health.Current / max : candidate.health.Current;
            if (value < bestValue)
            {
                bestValue = value;
                best = candidate;
            }
        }

        return best;
    }
}
