using UnityEngine;

public class PlayerDeckButton : MonoBehaviour
{
    [SerializeField] private PlayerDeckPanel deckPanel;

    public void ToggleDeck()
    {
        var panel = ResolvePanel();
        if (panel == null)
            return;

        if (panel.gameObject.activeSelf)
            panel.HidePanel();
        else
            panel.ShowPanel();
    }

    public void ShowDeck()
    {
        var panel = ResolvePanel();
        if (panel == null)
            return;

        panel.ShowPanel();
    }

    public void HideDeck()
    {
        var panel = ResolvePanel();
        if (panel == null)
            return;

        panel.HidePanel();
    }

    private PlayerDeckPanel ResolvePanel()
    {
        if (deckPanel == null)
            deckPanel = FindFirstObjectByType<PlayerDeckPanel>();

        if (deckPanel == null)
            Debug.LogError("PlayerDeckButton missing PlayerDeckPanel reference.", this);

        return deckPanel;
    }
}
