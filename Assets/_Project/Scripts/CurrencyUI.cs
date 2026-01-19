using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [Serializable]
    public struct CurrencyTextEntry
    {
        public CurrencyType type;
        public TMP_Text text;
    }

    [SerializeField] private List<CurrencyTextEntry> entries = new();

    private CurrencyManager _manager;

    private void OnEnable()
    {
        _manager = CurrencyManager.Instance;
        _manager.CurrencyChanged += HandleCurrencyChanged;
        RefreshAll();
    }

    private void OnDisable()
    {
        if (_manager != null)
            _manager.CurrencyChanged -= HandleCurrencyChanged;
    }

    private void HandleCurrencyChanged(CurrencyType type, int amount)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry.type != type || entry.text == null)
                continue;

            entry.text.text = amount.ToString();
            return;
        }
    }

    public void RefreshAll()
    {
        if (_manager == null)
            return;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry.text == null)
                continue;

            entry.text.text = _manager.Get(entry.type).ToString();
        }
    }
}
