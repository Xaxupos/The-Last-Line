using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Target Selectors/Closest")]
public class ClosestUnitSelector : UnitTargetSelector
{
    public override Unit SelectTarget(Unit self, IReadOnlyList<Unit> candidates)
    {
        if (self == null || candidates == null)
            return null;

        Unit best = null;
        float bestDistance = float.MaxValue;
        Vector3 origin = self.transform.position;

        for (int i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates[i];
            if (!IsCandidateValid(self, candidate))
                continue;

            float distance = (candidate.transform.position - origin).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }

        return best;
    }
}
