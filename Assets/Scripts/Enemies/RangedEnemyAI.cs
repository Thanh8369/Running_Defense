using System;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private Transform firePoint;

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
        if (stats.projectilePrefab == null || firePoint == null) return;
        if (currentTarget == null) return;

        GameObject proj = Instantiate(stats.projectilePrefab, firePoint.position, firePoint.rotation);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();

        p?.Initialize(currentTarget, stats.attackDamage, firePoint.right);
    }
}
