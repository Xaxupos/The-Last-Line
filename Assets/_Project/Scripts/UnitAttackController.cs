using System;
using UnityEngine;

public class UnitAttackController : MonoBehaviour
{
    [SerializeField] private Unit owner;
    [SerializeField] private float minAttackInterval = 0.05f;

    private float _nextAttackTime;
    private Unit _pendingTarget;
    private float _pendingDamage;
    private bool _hasPendingHit;

    public event Action<Unit, Unit> OnAttack;
    public event Action<Unit, Unit, float> OnHit;

    private void Awake()
    {
        if (owner.brain == null || owner.statsComponent == null)
        {
            Debug.LogError("UnitAttackController missing required references.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!enabled)
            return;

        if (owner.IsDead)
            return;

        var brain = owner.brain;
        var stats = owner.statsComponent;

        var target = brain.CurrentTarget;
        if (!IsTargetValid(target))
            return;

        float range = Mathf.Max(0f, stats.GetFinal(StatId.AttackRange));
        if (range <= 0f)
            return;

        float distanceSq = (target.transform.position - owner.transform.position).sqrMagnitude;
        if (distanceSq > range * range)
            return;

        if (Time.time < _nextAttackTime)
            return;

        ExecuteAttack(target);
    }

    private void ExecuteAttack(Unit target)
    {
        var stats = owner.statsComponent;

        float delay = Mathf.Max(minAttackInterval, stats.GetFinal(StatId.AttackDelayInSeconds));
        _nextAttackTime = Time.time + delay;

        owner.animator?.PlayAttack();
        OnAttack?.Invoke(owner, target);
        QueuePendingHit(target, Mathf.Max(0f, stats.GetFinal(StatId.Damage)));
    }

    private bool IsTargetValid(Unit target)
    {
        if (target == null || !target.isActiveAndEnabled)
            return false;

        if (target.health != null && target.health.Current <= 0f)
            return false;

        return true;
    }

    private void QueuePendingHit(Unit target, float damage)
    {
        _pendingTarget = target;
        _pendingDamage = damage;
        _hasPendingHit = damage > 0f;
    }

    //Need to call via animation event
    public void OnAttackHit()
    {
        if (!_hasPendingHit || owner.IsDead)
        {
            ClearPendingHit();
            return;
        }

        if (!IsTargetValid(_pendingTarget))
        {
            ClearPendingHit();
            return;
        }

        _pendingTarget.health?.TakeDamage(_pendingDamage);
        OnHit?.Invoke(owner, _pendingTarget, _pendingDamage);
        ClearPendingHit();
    }

    private void ClearPendingHit()
    {
        _pendingTarget = null;
        _pendingDamage = 0f;
        _hasPendingHit = false;
    }

    private void OnDisable()
    {
        ClearPendingHit();
    }
}
