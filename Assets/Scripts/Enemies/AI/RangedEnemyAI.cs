using System;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private Transform firePoint;
    private ProjectileData currentProjectileData;

    protected override void Start()
    {
        base.Start();
        // Set default projectile variant
        if (stats.projectilePrefabs.Count > 0)
        {
            currentProjectileData = stats.projectilePrefabs[0];
        }
    }

    protected override void SetupBT()
    {
        attackType = AttackType.Ranged;

        rootNode = new BTSelector(new List<BTNode>
        {
            // Player
            new BTSequence(new List<BTNode>
            {
                new BTCondition(() => DistanceToTarget(player) <= stats.detectionRange && currentFocusTime <= 0),

                new BTSelector(new List<BTNode>
                {
                    // Bắn player
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) <= stats.attackRange),
                        new BTAction(() => RotateToTarget(player)),
                        new BTAction(() => AttackTarget(player))
                    }),

                    // Tiến đến range bắn
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) > stats.attackRange),
                        new BTAction(() => RotateToTarget(player)),
                        new BTAction(() => MoveToTarget(player))
                    })
                })
            }),

            // Tower
            new BTSelector(new List<BTNode>
            {
                // Bắn tower
                new BTSequence(new List<BTNode>
                {
                    new BTCondition(() => DistanceToTarget(tower) <= stats.attackRange),
                    new BTAction(() => RotateToTarget(tower)),
                    new BTAction(() =>
                    {
                        if(currentFocusTime <= 0f) currentFocusTime = stats.attackCooldown;
                        return AttackTarget(tower);
                    })
                }),

                // Tiến gần tower
                new BTSequence(new List<BTNode>
                {
                    new BTAction(() => RotateToTarget(tower)),
                    new BTAction(() => MoveToTarget(tower))
                })
            })
        });
    }

    // Animation event
    public void FireProjectile()
    {
        if (currentTarget == null) return;

        if (currentProjectileData.prefab == null || firePoint == null) return;

        GameObject proj = PoolManager.Instance.Get(currentProjectileData.prefab, firePoint.position, firePoint.rotation);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();

        if (p != null)
        {
            float finalDamage = currentProjectileData.customDamage > 0
                ? currentProjectileData.customDamage
                : stats.attackDamage;

            p.Initialize(currentTarget, finalDamage, firePoint.right);
        }
    }

    public void SetProjectileVariant(int index)
    {
        if (index >= 0 && index < stats.projectilePrefabs.Count)
        {
            currentProjectileData = stats.projectilePrefabs[index];
        }
    }
}
