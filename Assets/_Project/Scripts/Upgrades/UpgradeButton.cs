using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeDefinition upgrade;
    [SerializeField] private int stacksPerPurchase = 1;
    [SerializeField] private bool startRevealed = true;
    [SerializeField] private List<UpgradeButton> connected = new();

    private Button _button;
    private bool _revealed;
    private bool _skipAutoReveal;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (!_skipAutoReveal)
            SetRevealed(startRevealed);
    }

    public void TryPurchase()
    {
        if (!_revealed || upgrade == null)
        {
            return;
        }

        if (!RunStateManager.HasInstance)
        {
            return;
        }

        int currentStacks = RunStateManager.Instance.GetStacks(upgrade);
        int maxStacks = upgrade.maxStacks > 0 ? upgrade.maxStacks : int.MaxValue;
        if (currentStacks >= maxStacks)
        {
            RefreshInteractable();
            return;
        }

        int purchaseStacks = Mathf.Max(1, stacksPerPurchase);
        if (!CanAfford())
        {
            return;
        }

        if (upgrade.costs != null && upgrade.costs.Length > 0)
        {
            if (!CurrencyManager.Instance.TrySpend(upgrade.costs))
            {
                return;
            }
        }

        RunStateManager.Instance.AddUpgrade(upgrade, purchaseStacks);
        RevealConnected();
        RefreshInteractable();
    }

    public void SetRevealed(bool revealed)
    {
        _skipAutoReveal = true;
        _revealed = revealed;
        gameObject.SetActive(revealed);

        RefreshInteractable();
    }

    public void RefreshInteractable()
    {
        if (_button == null)
            return;

        _button.interactable = _revealed && CanStackMore();
    }

    private void RevealConnected()
    {
        if (connected == null)
            return;

        for (int i = 0; i < connected.Count; i++)
        {
            var node = connected[i];
            if (node == null)
                continue;

            node.SetRevealed(true);
            node.RefreshInteractable();
        }
    }

    private bool CanAfford()
    {
        if (upgrade == null)
            return false;

        if (upgrade.costs == null || upgrade.costs.Length == 0)
            return true;

        if (!CurrencyManager.HasInstance)
            return false;

        return CurrencyManager.Instance.CanAfford(upgrade.costs);
    }

    private bool CanStackMore()
    {
        if (upgrade == null || !RunStateManager.HasInstance)
            return false;

        int currentStacks = RunStateManager.Instance.GetStacks(upgrade);
        int maxStacks = upgrade.maxStacks > 0 ? upgrade.maxStacks : int.MaxValue;
        return currentStacks < maxStacks;
    }
}
