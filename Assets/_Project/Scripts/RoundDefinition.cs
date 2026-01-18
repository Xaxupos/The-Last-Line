using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Rounds/Round Definition")]
public class RoundDefinition : ScriptableObject
{
    public int roundIndex;
    public float timeLimitSeconds;

    public EnemySpawnEntry[] enemies;
}
