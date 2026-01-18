using UnityEngine;

[CreateAssetMenu(menuName = "Game/Status Effects/Status Effect Definition")]
public class StatusEffectDefinition : ScriptableObject
{
    [Header("Identity")]
    public string effectId;
    public bool isDebuff;

    [Header("Timing")]
    public float durationSeconds = 5f;
    public float tickIntervalSeconds = 1f;

    [Header("Tick")]
    public float tickValue = 1f;
    public bool tickIsHeal;

    [Header("Stat Modifiers")]
    public StatModifierEntry[] statModifiers;

    [Header("Rules")]
    public bool canBeDispelled = true;
    public bool removeOnSourceDeath = true;

    [Header("Feedback")]
    public GameObject tickVfxPrefab;
    public AudioClip tickSfx;

    public string GetKey()
    {
        return string.IsNullOrWhiteSpace(effectId) ? name : effectId;
    }
}
