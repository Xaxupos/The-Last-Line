using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private List<BoxCollider> spawnAreas = new();

    private Coroutine _spawnRoutine;
    private bool _running;

    public void StartSpawning(RoundRuntimeState runtime)
    {
        if (runtime == null || runtime.BaseDefinition == null)
        {
            Debug.LogError("EnemySpawnerController.StartSpawning called with null runtime.", this);
            return;
        }

        if (!HasValidSpawnAreas())
        {
            Debug.LogError("EnemySpawnerController has no valid spawn areas.", this);
            return;
        }

        StopSpawning();
        _running = true;
        _spawnRoutine = StartCoroutine(Spawn(runtime));
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

    private IEnumerator Spawn(RoundRuntimeState runtime)
    {
        var definition = runtime.BaseDefinition;
        if (definition.enemyPool == null || definition.enemyPool.Length == 0)
        {
            Debug.LogWarning("EnemySpawnerController has an empty enemy pool.", this);
            yield break;
        }

        while (_running)
        {
            float rate = runtime.SpawnRate;
            if (rate <= 0f)
            {
                yield return null;
                continue;
            }

            var enemyPrefab = PickEnemy(definition.enemyPool);
            if (enemyPrefab != null)
                SpawnEnemy(enemyPrefab);

            float interval = 1f / rate;
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnEnemy(Unit def)
    {
        if (!TryGetRandomSpawnPosition(out var position))
            return;

        var spawnedUnit = Instantiate(def, position, Quaternion.identity);
        spawnedUnit.SetTeam(Team.Enemy);
    }

    private Unit PickEnemy(EnemySpawnOption[] pool)
    {
        float totalWeight = 0f;
        for (int i = 0; i < pool.Length; i++)
        {
            var entry = pool[i];
            if (entry.enemyPrefab == null || entry.weight <= 0f)
                continue;
            totalWeight += entry.weight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.value * totalWeight;
        float acc = 0f;
        for (int i = 0; i < pool.Length; i++)
        {
            var entry = pool[i];
            if (entry.enemyPrefab == null || entry.weight <= 0f)
                continue;
            acc += entry.weight;
            if (roll <= acc)
                return entry.enemyPrefab;
        }

        return null;
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
