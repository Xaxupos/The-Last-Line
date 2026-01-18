using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class Unit : MonoBehaviour
{
    public Team team;
    public HealthComponent health;
    public StatsComponent statsComponent;
    public UnitDefinition definition;

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
        UnitsManager.Instance.NotifyUnitDied(this);
        Destroy(gameObject);
    }
}
