using System;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 15f;

    protected override void SetupBT()
    {
        attackType = AttackType.Ranged;

        rootNode = new BTSelector(new List<BTNode>
        {
            // Player
            new BTSequence(new List<BTNode>
            {
                new BTCondition(() => DistanceToTarget(player) <= detectionRange && currentFocusTime <= 0),

                new BTSelector(new List<BTNode>
                {
                    // Bắn player
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) <= attackRange),
                        new BTAction(() => RotateToTarget(player)),
                        new BTAction(() => AttackTarget(player))
                    }),

                    // Tiến đến range bắn
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) > attackRange),
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
                    new BTCondition(() => DistanceToTarget(tower) <= attackRange),
                    new BTAction(() => RotateToTarget(tower)),
                    new BTAction(() =>
                    {
                        if(currentFocusTime <= 0f) currentFocusTime = attackCooldown;
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
        if (projectilePrefab == null || firePoint == null) return;
        if (currentTarget == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile p = proj.GetComponent<Projectile>();

        p?.Initialize(currentTarget, attackDamage, projectileSpeed, firePoint.right);
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Vẽ attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
