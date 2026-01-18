using System;

[Serializable]
public struct EnemySpawnEntry
{
    public Unit enemyPrefab;
    public int count;
    public float interval;
}