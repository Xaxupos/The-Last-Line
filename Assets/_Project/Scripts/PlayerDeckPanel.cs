using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDeckPanel : UIPanel
{
    [Serializable]
    public class SlotIndexEvent : UnityEvent<int> { }

    [SerializeField] private List<PlayerDeckSlotUI> slotViews = new();
    [SerializeField] private bool removeUnitOnOccupiedClick = true;
    [SerializeField] private SlotIndexEvent OnEmptySlotClicked;
    [SerializeField] private SlotIndexEvent OnOccupiedSlotClicked;

    private PlayerDeck _deck;
    private bool _initialized;
    private bool _warnedMissingSlots;

    private void OnEnable()
    {
        InitializeSlots();
        BindDeck();
        Refresh();
    }

    private void OnDisable()
    {
        UnbindDeck();
    }

    public void Refresh()
    {
        if (_deck == null)
            return;

        var slots = _deck.Slots;
        if (slots == null)
            return;

        if (!_warnedMissingSlots && slots.Count > slotViews.Count)
        {
            Debug.LogWarning("PlayerDeckPanel has fewer slot views than deck slots.", this);
            _warnedMissingSlots = true;
        }

        for (int i = 0; i < slotViews.Count; i++)
        {
            var view = slotViews[i];
            if (view == null)
                continue;

            bool inDeck = i < slots.Count;
            view.gameObject.SetActive(inDeck);
            if (!inDeck)
                continue;

            view.SetUnit(slots[i]);
            view.SetInteractable(true);
        }
    }

    public void HandleSlotClicked(int slotIndex)
    {
        if (_deck == null)
            return;

        var unit = _deck.GetUnitAtSlot(slotIndex);
        if (unit == null)
        {
            OnEmptySlotClicked?.Invoke(slotIndex);
            return;
        }

        OnOccupiedSlotClicked?.Invoke(slotIndex);
        if (removeUnitOnOccupiedClick)
            _deck.RemoveUnit(slotIndex);
    }

    private void InitializeSlots()
    {
        if (_initialized)
            return;

        for (int i = 0; i < slotViews.Count; i++)
        {
            var view = slotViews[i];
            if (view == null)
                continue;

            view.Initialize(this, i);
        }

        _initialized = true;
    }

    private void BindDeck()
    {
        _deck = PlayerDeck.InstanceOrNull;
        if (_deck == null)
        {
            Debug.LogError("PlayerDeckPanel could not find PlayerDeck.", this);
            return;
        }

        if (_deck.OnDeckChanged != null)
            _deck.OnDeckChanged.AddListener(Refresh);
    }

    private void UnbindDeck()
    {
        if (_deck == null)
            return;

        if (_deck.OnDeckChanged != null)
            _deck.OnDeckChanged.RemoveListener(Refresh);

        _deck = null;
    }
}
