using System.Collections.Generic;
using UnityEngine;

public class EvilMageAI : RangedEnemyAI
{
    [SerializeField] private List<AnimationFirepointProjectileMap> attackMappings = new List<AnimationFirepointProjectileMap>();
    private AnimationFirepointProjectileMap currentAttackMapping;
    private GameObject currentProjectile;
    private float projectileScaleStartTime;

    public void OnAttackAnimationSet(string triggerName)
    {
        currentAttackMapping = GetMappingForAnimation(triggerName);
        if (currentAttackMapping != null)
            SetProjectileVariant(currentAttackMapping.projectileVariantIndex);
    }

    private AnimationFirepointProjectileMap GetMappingForAnimation(string triggerName)
    {
        foreach (var map in attackMappings)
        {
            if (map.animationTriggerName == triggerName && map.firePoint != null)
                return map;
        }
        return null;
    }

    public override void FireProjectile()
    {
        if (currentTarget == null || currentAttackMapping == null) return;

        var projectileVariants = stats.projectilePrefabs;
        if (projectileVariants.Count == 0) return;

        int variantIndex = Mathf.Clamp(currentAttackMapping.projectileVariantIndex, 0, projectileVariants.Count - 1);
        ProjectileData projData = projectileVariants[variantIndex];

        if (projData.prefab == null) return;

        currentProjectile = PoolManager.Instance.Get(projData.prefab, currentAttackMapping.firePoint.position, currentAttackMapping.firePoint.rotation);
        EnemyProjectile p = currentProjectile.GetComponent<EnemyProjectile>();

        if (p != null)
        {
            float finalDamage = projData.customDamage > 0 ? projData.customDamage : stats.attackDamage;
            p.Initialize(currentTarget, finalDamage, currentAttackMapping.firePoint.forward);
            p.StopMoving();
        }

        if (currentAttackMapping.scaleUpDuration > 0)
        {
            currentProjectile.transform.localScale = Vector3.zero;
            projectileScaleStartTime = Time.time;
        }
    }

    public void LaunchCurrentProjectile()
    {
        if (currentProjectile == null) return;
        EnemyProjectile p = currentProjectile.GetComponent<EnemyProjectile>();
        p?.StartMoving();
    }

    protected override void Update()
    {
        base.Update();

        if (currentProjectile != null && currentProjectile.activeInHierarchy &&
            currentAttackMapping != null && currentAttackMapping.scaleUpDuration > 0)
        {
            float elapsedTime = Time.time - projectileScaleStartTime;
            float progress = Mathf.Clamp01(elapsedTime / currentAttackMapping.scaleUpDuration);
            currentProjectile.transform.localScale = Vector3.one * (progress * currentAttackMapping.scaleUpMax);
        }
    }
}
