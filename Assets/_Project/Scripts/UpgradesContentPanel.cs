using System.Collections.Generic;
using UnityEngine;

public class UpgradesContentPanel : UIPanel
{
    public List<UIPanel> upgradeContents;

    public void CloseAllContents()
    {
        foreach(var content in upgradeContents) content.HidePanel();
    }
}