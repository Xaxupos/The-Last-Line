using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public UIFader uiFader;

    public virtual void ShowPanel()
    {
        uiFader.FadeIn();
    }

    public virtual void HidePanel()
    {
        uiFader.FadeOut();
    }
}