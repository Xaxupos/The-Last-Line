using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Target Selectors/Best AOE Cluster")]
public class BestAoeClusterSelector : UnitTargetSelector
{
    [SerializeField] private float radius = 2f;

    public override Unit SelectTarget(Unit self, IReadOnlyList<Unit> candidates)
    {
        if (self == null || candidates == null || candidates.Count == 0)
            return null;

        Unit best = null;
        int bestCount = -1;
        float bestDistance = float.MaxValue;
        float radiusSq = radius * radius;
        Vector3 origin = self.transform.position;

        for (int i = 0; i < candidates.Count; i++)
        {
            var candidate = candidates[i];
            if (!IsCandidateValid(self, candidate))
                continue;

            Vector3 candidatePos = candidate.transform.position;
            int count = 0;

            for (int j = 0; j < candidates.Count; j++)
            {
                var neighbor = candidates[j];
                if (!IsCandidateValid(self, neighbor))
                    continue;

                float distSq = (neighbor.transform.position - candidatePos).sqrMagnitude;
                if (distSq <= radiusSq)
                    count++;
            }

            float distance = (candidatePos - origin).sqrMagnitude;
            if (count > bestCount || (count == bestCount && distance < bestDistance))
            {
                bestCount = count;
                bestDistance = distance;
                best = candidate;
            }
        }

        return best;
    }
}
