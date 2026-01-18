using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Upgrades/Upgrade Definition")]
public class UpgradeDefinition : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public int maxStacks = 1;
    public List<UpgradeEffectDefinition> effects = new();
}
