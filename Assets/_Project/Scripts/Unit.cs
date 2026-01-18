using UnityEngine;

public class Unit : MonoBehaviour
{
    public Team team;
    public HealthComponent health;
    public StatsComponent statsComponent;
    public UnitDefinition definition;
    public UnitAnimator animator;
    public UnitBrain brain;
    public UnitAttackController attackController;
    public StatusEffectController statusEffects;
    public UnitOnHitEffectController onHitEffects;
    public UnitMovementController movementController;
    [SerializeField] private float deathAnimationDuration = 1f;

    public bool IsDead { get; private set; }

    private void Awake()
    {
        if (health != null && health.owner == null)
            health.owner = this;

        if (definition != null && statsComponent != null)
            statsComponent.SetBaseStats(definition.baseStats, true);
    }

    private void OnEnable()
    {
        UnitsManager.Instance.Register(this);
        if (health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (UnitsManager.HasInstance)
            UnitsManager.Instance.Unregister(this);

        if (health != null)
            health.OnDied -= HandleDeath;
    }

    public void SetTeam(Team t)
    {
        if (team == t) return;

        team = t;

        if (!isActiveAndEnabled || !UnitsManager.HasInstance)
            return;

        UnitsManager.Instance.Unregister(this);
        UnitsManager.Instance.Register(this);
    }

    private void HandleDeath()
    {
        if (IsDead)
            return;

        IsDead = true;
        if (brain != null)
            brain.enabled = false;
        if (attackController != null)
            attackController.enabled = false;
        if (movementController != null)
            movementController.enabled = false;
        if (onHitEffects != null)
            onHitEffects.enabled = false;
        if (statusEffects != null)
        {
            statusEffects.ClearAll();
            statusEffects.enabled = false;
        }
        if (animator != null)
            animator.PlayDeath();

        UnitsManager.Instance.NotifyUnitDied(this);
    }
}
