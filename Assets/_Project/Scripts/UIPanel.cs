using UnityEngine;

[RequireComponent(typeof(UIFader))]
public class UIPanel : MonoBehaviour
{
    public UIFader uiFader;

    void Awake()
    {
        uiFader = GetComponent<UIFader>();
    }

    public virtual void ShowPanel()
    {
        uiFader.FadeIn();
    }

    public virtual void HidePanel()
    {
        uiFader.FadeOut();
    }
}