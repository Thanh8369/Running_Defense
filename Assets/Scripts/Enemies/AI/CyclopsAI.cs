using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationFirepointProjectileMap
{
    public string animationTriggerName;
    public Transform firePoint;
    public GameObject projectilePrefab;
}

public class CyclopsAI : RangedEnemyAI
{
    [SerializeField] private List<AnimationFirepointProjectileMap> attackMappings = new List<AnimationFirepointProjectileMap>();

    private AnimationFirepointProjectileMap currentAttackMapping;

    public void OnAttackAnimationSet(string triggerName)
    {
        currentAttackMapping = GetMappingForAnimation(triggerName);
    }

    private AnimationFirepointProjectileMap GetMappingForAnimation(string triggerName)
    {
        foreach (var map in attackMappings)
        {
            if (map.animationTriggerName == triggerName && map.firePoint != null && map.projectilePrefab != null)
                return map;
        }
        return null;
    }

    // Animation event
    public new void FireProjectile()
    {
        if (currentTarget == null) return;

        GameObject proj = PoolManager.Instance.Get(
            currentAttackMapping.projectilePrefab,
            currentAttackMapping.firePoint.position,
            currentAttackMapping.firePoint.rotation
        );

        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();
        p?.Initialize(currentTarget, stats.attackDamage, currentAttackMapping.firePoint.right);
    }
}
