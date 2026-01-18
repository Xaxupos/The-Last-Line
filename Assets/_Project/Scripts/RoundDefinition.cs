using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Rounds/Round Definition")]
public class RoundDefinition : ScriptableObject
{
    public float timeLimitSeconds = 30f;
    public float baseSpawnRate = 1f;
    public EnemySpawnOption[] enemyPool;
}
