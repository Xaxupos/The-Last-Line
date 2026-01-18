using UnityEngine;

[CreateAssetMenu(menuName = "Game/On Hit Actions/AOE Explosion")]
public class AoeExplosionOnHitAction : OnHitActionDefinition
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private bool includePrimaryTarget;
    [SerializeField] private StatusEffectDefinition statusEffect;
    [Range(0f, 100f)] [SerializeField] private float statusEffectChancePercent = 100f;
    [SerializeField] private float statusEffectStrength = 1f;

    public override void Apply(Unit attacker, Unit target, float baseDamage)
    {
        if (attacker == null || target == null || !UnitsManager.HasInstance)
            return;

        float radiusSq = radius * radius;
        Vector3 center = target.transform.position;
        float damage = Mathf.Max(0f, baseDamage * damageMultiplier);

        var candidates = attacker.team == Team.Player
            ? UnitsManager.Instance.Enemies
            : UnitsManager.Instance.Players;

        for (int i = 0; i < candidates.Count; i++)
        {
            var unit = candidates[i];
            if (unit == null || unit.IsDead || !unit.isActiveAndEnabled)
                continue;

            if (!includePrimaryTarget && unit == target)
                continue;

            if ((unit.transform.position - center).sqrMagnitude > radiusSq)
                continue;

            if (damage > 0f && unit.health != null)
                unit.health.TakeDamage(damage);

            if (statusEffect == null || unit.statusEffects == null)
                continue;

            if (statusEffectChancePercent > 0f && Roll(statusEffectChancePercent))
            {
                float strength = statusEffectStrength <= 0f ? 1f : statusEffectStrength;
                unit.statusEffects.Apply(statusEffect, attacker, strength);
            }
        }
    }

    private static bool Roll(float chancePercent)
    {
        return Random.value * 100f <= chancePercent;
    }
}
