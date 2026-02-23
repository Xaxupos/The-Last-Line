using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitsSpawner : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> spawnAreas = new();

    public List<Unit> SpawnFromDeck()
    {
        var spawned = new List<Unit>();
        var deck = PlayerDeck.InstanceOrNull;
        if (deck == null)
        {
            Debug.LogError("PlayerUnitsSpawner has no PlayerDeck available.", this);
            return spawned;
        }

        var slots = deck.Slots;
        if (slots == null || slots.Count == 0)
        {
            Debug.LogWarning("PlayerUnitsSpawner deck has no slots.", this);
            return spawned;
        }

        if (!HasValidSpawnAreas())
        {
            Debug.LogError("PlayerUnitsSpawner has no valid spawn areas.", this);
            return spawned;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            var prefab = slots[i];
            if (prefab == null)
                continue;

            if (!TryGetRandomSpawnPosition(out var position))
                continue;

            var rotation = Quaternion.identity;
            var unit = Instantiate(prefab, position, rotation);
            unit.SetTeam(Team.Player);

            if (RunStateManager.HasInstance)
                RunStateManager.Instance.ApplyToUnit(unit);

            spawned.Add(unit);
        }

        return spawned;
    }

    private bool HasValidSpawnAreas()
    {
        if (spawnAreas == null || spawnAreas.Count == 0)
            return false;

        foreach (var area in spawnAreas)
            if (area != null)
                return true;

        return false;
    }

    private bool TryGetRandomSpawnPosition(out Vector3 position)
    {
        position = default;

        if (!HasValidSpawnAreas())
            return false;

        BoxCollider area = null;
        for (int i = 0; i < 10; i++)
        {
            var candidate = spawnAreas[Random.Range(0, spawnAreas.Count)];
            if (candidate != null)
            {
                area = candidate;
                break;
            }
        }

        if (area == null)
            return false;

        Vector3 halfSize = area.size * 0.5f;
        float x = Random.Range(-halfSize.x, halfSize.x);
        float z = Random.Range(-halfSize.z, halfSize.z);
        float y = area.center.y - halfSize.y;

        Vector3 localPoint = new Vector3(area.center.x + x, y, area.center.z + z);
        position = area.transform.TransformPoint(localPoint);
        return true;
    }
}
