using UnityEngine;
using VInspector;

[CreateAssetMenu(menuName = "Game/Units/Unit Definition")]
public class UnitDefinition : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public StatBaseEntry[] baseStats;

    [Header("Loot")]
    public SerializedDictionary<CurrencyType, Vector2Int> lootTable = new();
}
