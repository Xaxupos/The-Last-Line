using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBrain : MonoBehaviour
{
    [SerializeField] private Unit owner;
    [SerializeField] private UnitTargetSelector targetSelector;
    [SerializeField] private float retargetInterval = 0.2f;

    private Unit _currentTarget;
    private float _nextRetargetTime;

    public Unit CurrentTarget => _currentTarget;
    public UnitTargetSelector TargetSelector => targetSelector;
    public event Action<Unit> OnTargetChanged;

    private void Awake()
    {
        if (targetSelector == null)
        {
            Debug.LogError("UnitBrain missing required references.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _nextRetargetTime = 0f;
        AcquireTarget();
    }

    private void Update()
    {
        if (Time.time < _nextRetargetTime && IsTargetValid(_currentTarget))
            return;

        AcquireTarget();
    }

    public void ForceRetarget()
    {
        _nextRetargetTime = 0f;
        AcquireTarget();
    }

    public void SetTargetSelector(UnitTargetSelector selector)
    {
        targetSelector = selector;
        ForceRetarget();
    }

    private void AcquireTarget()
    {
        if (owner.IsDead || !UnitsManager.HasInstance || targetSelector == null)
        {
            SetTarget(null);
            return;
        }

        var candidates = GetCandidates();
        var target = targetSelector.SelectTarget(owner, candidates);
        SetTarget(target);

        _nextRetargetTime = Time.time + Mathf.Max(0.02f, retargetInterval);
    }

    private IReadOnlyList<Unit> GetCandidates()
    {
        if (!UnitsManager.HasInstance || owner == null)
            return Array.Empty<Unit>();

        bool targetEnemies = targetSelector.TargetTeam == TargetTeam.Enemy;
        bool ownerIsPlayer = owner.team == Team.Player;

        if (targetEnemies)
            return ownerIsPlayer ? UnitsManager.Instance.Enemies : UnitsManager.Instance.Players;

        return ownerIsPlayer ? UnitsManager.Instance.Players : UnitsManager.Instance.Enemies;
    }

    private void SetTarget(Unit target)
    {
        if (target == _currentTarget)
            return;

        _currentTarget = target;
        OnTargetChanged?.Invoke(_currentTarget);
    }

    private bool IsTargetValid(Unit target)
    {
        if (target == null || !target.isActiveAndEnabled)
            return false;

        if (target.IsDead)
            return false;

        if (target.health != null && target.health.Current <= 0f)
            return false;

        return true;
    }
}
