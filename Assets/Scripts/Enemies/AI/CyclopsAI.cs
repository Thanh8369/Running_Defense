using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationFirepointProjectileMap
{
    public string animationTriggerName;
    public Transform firePoint;
    public int projectileVariantIndex = 0;
}

public class CyclopsAI : RangedEnemyAI
{
    [SerializeField] private List<AnimationFirepointProjectileMap> attackMappings = new List<AnimationFirepointProjectileMap>();
    private AnimationFirepointProjectileMap currentAttackMapping;

    public void OnAttackAnimationSet(string triggerName)
    {
        currentAttackMapping = GetMappingForAnimation(triggerName);
        
        if (currentAttackMapping != null)
        {
            // Switch projectile variant theo animation
            SetProjectileVariant(currentAttackMapping.projectileVariantIndex);
        }
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

    public new void FireProjectile()
    {
        if (currentTarget == null || currentAttackMapping == null) return;

        // Lấy projectile variant từ mapping
        var projectileVariants = stats.projectilePrefabs;
        if (projectileVariants.Count == 0) return;

        int variantIndex = Mathf.Clamp(currentAttackMapping.projectileVariantIndex, 0, projectileVariants.Count - 1);
        ProjectileData projData = projectileVariants[variantIndex];

        if (projData.prefab == null) return;

        GameObject proj = PoolManager.Instance.Get(projData.prefab, currentAttackMapping.firePoint.position, currentAttackMapping.firePoint.rotation);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();

        if (p != null)
        {
            float finalDamage = projData.customDamage > 0 ? projData.customDamage : stats.attackDamage;
            p.Initialize(currentTarget, finalDamage, currentAttackMapping.firePoint.forward);
        }
    }
}
