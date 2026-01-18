using System;
using UnityEngine;

[RequireComponent(typeof(StatsComponent))]
public class HealthComponent : MonoBehaviour
{
    public Unit owner;
    public event Action OnDied;

    private StatsComponent _stats;
    private float _currentHp;

    private void Awake()
    {
        _stats = owner.statsComponent;

        if (_stats == null)
        {
            Debug.LogError("HealthComponent missing required references.", this);
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (_stats == null)
            return;

        if (_currentHp <= 0f)
            _currentHp = Max;
    }

    private void OnEnable()
    {
        if (_stats != null)
            _stats.OnStatChanged += HandleStatChanged;
    }

    private void OnDisable()
    {
        if (_stats != null)
            _stats.OnStatChanged -= HandleStatChanged;
    }

    public void TakeDamage(float dmg)
    {
        if (_currentHp <= 0f)
            return;

        _currentHp -= Mathf.Max(0f, dmg);
        if (_currentHp <= 0f)
            OnDied?.Invoke();
    }

    public float Current => _currentHp;
    public float Max => _stats != null ? _stats.GetFinal(StatId.MaxHp) : 0f;

    private void HandleStatChanged(StatId id)
    {
        if (id != StatId.MaxHp)
            return;

        float max = Max;
        if (_currentHp > max)
            _currentHp = max;
    }
}
