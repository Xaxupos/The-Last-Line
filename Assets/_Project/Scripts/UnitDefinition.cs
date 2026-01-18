using UnityEngine;

[CreateAssetMenu(menuName = "Game/Units/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public StatBaseEntry[] baseStats;
}
