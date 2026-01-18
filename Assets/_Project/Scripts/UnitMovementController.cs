using UnityEngine;
using UnityEngine.AI;

public class UnitMovementController : MonoBehaviour
{
    [SerializeField] private Unit owner;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private bool updateAnimatorSpeed = true;

    private void Awake()
    {
        if (agent == null || owner.brain == null || owner.statsComponent == null)
        {
            Debug.LogError("UnitMovementController missing required references.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!enabled)
            return;

        if (owner.IsDead)
        {
            StopMovement();
            UpdateAnimatorSpeed();
            return;
        }

        if (!agent.isOnNavMesh)
            return;

        var stats = owner.statsComponent;
        var brain = owner.brain;

        agent.speed = Mathf.Max(0f, stats.GetFinal(StatId.MoveSpeed));

        var target = brain.CurrentTarget;
        if (target == null || target.IsDead || !target.isActiveAndEnabled)
        {
            StopMovement();
            UpdateAnimatorSpeed();
            return;
        }

        float attackRange = Mathf.Max(0f, stats.GetFinal(StatId.AttackRange));
        agent.stoppingDistance = attackRange;

        float distanceSq = (target.transform.position - owner.transform.position).sqrMagnitude;
        if (distanceSq <= attackRange * attackRange)
        {
            StopMovement();
            UpdateAnimatorSpeed();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(target.transform.position);
        UpdateAnimatorSpeed();
    }

    private void StopMovement()
    {
        agent.isStopped = true;
        if (agent.hasPath)
            agent.ResetPath();
    }

    private void UpdateAnimatorSpeed()
    {
        if (!updateAnimatorSpeed || owner.animator == null)
            return;

        owner.animator.SetSpeed(agent.velocity.magnitude);
    }
}
