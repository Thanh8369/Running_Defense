using System;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] private float keepDistance = 8f;
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
                    // Lùi nếu quá gần
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) < keepDistance * 0.5f),
                        new BTAction(() => MoveAwayFrom(player))
                    }),

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
                // Lùi nếu quá gần
                new BTSequence(new List<BTNode>
                {
                    new BTCondition(() => DistanceToTarget(tower) < keepDistance * 0.5f),
                    new BTAction(() => MoveAwayFrom(tower))
                }),

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

    private BTNode.NodeState MoveAwayFrom(Transform target)
    {
        if (target == null) return BTNode.NodeState.Failure;

        Vector3 dir = transform.position - target.position;
        dir.y = 0;

        rb.MovePosition(transform.position + dir.normalized * moveSpeed * Time.deltaTime);
        return BTNode.NodeState.Running;
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
}
