using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> spawnAreas = new();

    private Coroutine _spawnRoutine;
    private bool _running;

    public void StartSpawning(RoundDefinition round)
    {
        if (round == null)
        {
            Debug.LogError("EnemySpawnerController.StartSpawning called with null round.", this);
            return;
        }

        if (!HasValidSpawnAreas())
        {
            Debug.LogError("EnemySpawnerController has no valid spawn areas.", this);
            return;
        }

        StopSpawning();
        _running = true;
        _spawnRoutine = StartCoroutine(Spawn(round));
    }

    public void StopSpawning()
    {
        _running = false;
        if (_spawnRoutine != null)
        {
            StopCoroutine(_spawnRoutine);
            _spawnRoutine = null;
        }
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private IEnumerator Spawn(RoundDefinition round)
    {
        foreach (var e in round.enemies)
        {
            for (int i = 0; i < e.count; i++)
            {
                if (!_running)
                    yield break;

                if (e.enemyPrefab == null)
                {
                    Debug.LogWarning("EnemySpawnerController has a null enemy prefab entry.", this);
                    continue;
                }

                SpawnEnemy(e.enemyPrefab);
                yield return new WaitForSeconds(e.interval);
            }
        }
    }

    private void SpawnEnemy(Unit def)
    {
        if (!TryGetRandomSpawnPosition(out var position))
            return;

        var spawnedUnit = Instantiate(def, position, Quaternion.identity);
        spawnedUnit.SetTeam(Team.Enemy);
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
