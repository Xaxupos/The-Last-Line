using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class CurrencyManager : MonoBehaviourSingleton<CurrencyManager>
{
    [Serializable]
    public class CurrencyChangedEvent : UnityEvent<CurrencyType, int> { }

    [SerializeField] private CurrencyChangedEvent OnCurrencyChanged;
    public event Action<CurrencyType, int> CurrencyChanged;

    public SerializedDictionary<CurrencyType, int> Amounts = new();

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        if (Amounts == null)
            Amounts = new SerializedDictionary<CurrencyType, int>();

        var keys = new CurrencyType[Amounts.Count];
        int index = 0;
        foreach (var key in Amounts.Keys)
            keys[index++] = key;

        for (int i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            Amounts[key] = Mathf.Max(0, Amounts[key]);
        }
    }

    public int Get(CurrencyType type)
    {
        return Amounts != null && Amounts.TryGetValue(type, out var value) ? value : 0;
    }

    public void Add(CurrencyType type, int amount)
    {
        if (amount == 0)
            return;

        int current = Get(type);
        int next = Mathf.Max(0, current + amount);
        Amounts[type] = next;
        OnCurrencyChanged?.Invoke(type, next);
        CurrencyChanged?.Invoke(type, next);
    }

    public bool CanAfford(IReadOnlyList<CurrencyAmount> costs)
    {
        if (costs == null || costs.Count == 0)
            return true;

        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            if (Get(cost.type) < cost.amount)
                return false;
        }

        return true;
    }

    public bool TrySpend(IReadOnlyList<CurrencyAmount> costs)
    {
        if (!CanAfford(costs))
            return false;

        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];
            Add(cost.type, -cost.amount);
        }

        return true;
    }
}
