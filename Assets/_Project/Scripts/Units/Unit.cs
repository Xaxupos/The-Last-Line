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

        TryDropLoot();
        UnitsManager.Instance.NotifyUnitDied(this);
    }

    private void TryDropLoot()
    {
        if (team != Team.Enemy || definition == null)
            return;

        if (definition.lootTable == null || definition.lootTable.Count == 0)
            return;

        if (!CurrencyManager.HasInstance)
            return;

        foreach (var entry in definition.lootTable)
        {
            var range = entry.Value;
            int amount = Random.Range(range.x, range.y + 1);
            if (amount <= 0)
                continue;

            if (RunStateManager.HasInstance)
                amount = RunStateManager.Instance.ModifyLootAmount(entry.Key, amount);

            if (amount <= 0)
                continue;

            CurrencyManager.Instance.Add(entry.Key, amount);
        }
    }
}
