using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDeck : MonoBehaviourSingleton<PlayerDeck>
{
    [SerializeField] private int maxSlots = 1;
    [SerializeField] private List<Unit> slots = new();

    public UnityEvent OnDeckChanged;

    public int MaxSlots => maxSlots;
    public IReadOnlyList<Unit> Slots => slots;

    protected override void Awake()
    {
        base.Awake();
        if (OnDeckChanged == null)
            OnDeckChanged = new UnityEvent();
        EnsureSlots();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (maxSlots < 1)
            maxSlots = 1;

        EnsureSlots();
    }
#endif

    public void SetMaxSlots(int value)
    {
        int clamped = Mathf.Max(1, value);
        if (clamped == maxSlots)
            return;

        maxSlots = clamped;
        EnsureSlots();
        OnDeckChanged?.Invoke();
    }

    public void IncreaseMaxSlots(int amount)
    {
        if (amount <= 0)
            return;

        SetMaxSlots(maxSlots + amount);
    }

    public Unit GetUnitAtSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
            return null;

        return slots[slotIndex];
    }

    public bool TryAssignUnitToSlot(Unit unitPrefab, int slotIndex)
    {
        if (unitPrefab == null || !IsValidSlot(slotIndex))
            return false;

        if (slots[slotIndex] == unitPrefab)
            return true;

        slots[slotIndex] = unitPrefab;
        OnDeckChanged?.Invoke();
        return true;
    }

    public bool TryAddUnit(Unit unitPrefab)
    {
        if (unitPrefab == null)
            return false;

        int index = FindFirstEmptySlot();
        if (index < 0)
            return false;

        slots[index] = unitPrefab;
        OnDeckChanged?.Invoke();
        return true;
    }

    public bool RemoveUnit(int slotIndex)
    {
        if (!IsValidSlot(slotIndex) || slots[slotIndex] == null)
            return false;

        slots[slotIndex] = null;
        OnDeckChanged?.Invoke();
        return true;
    }

    public void ClearAll()
    {
        bool changed = false;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null)
            {
                slots[i] = null;
                changed = true;
            }
        }

        if (changed)
            OnDeckChanged?.Invoke();
    }

    public bool IsFull()
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i] == null)
                return false;

        return true;
    }

    public bool HasAnyUnits()
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i] != null)
                return true;

        return false;
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < slots.Count; i++)
            if (slots[i] == null)
                return i;

        return -1;
    }

    private bool IsValidSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < slots.Count;
    }

    private void EnsureSlots()
    {
        if (slots == null)
            slots = new List<Unit>();

        if (slots.Count < maxSlots)
        {
            int needed = maxSlots - slots.Count;
            for (int i = 0; i < needed; i++)
                slots.Add(null);
        }
        else if (slots.Count > maxSlots)
        {
            slots.RemoveRange(maxSlots, slots.Count - maxSlots);
        }
    }
}
