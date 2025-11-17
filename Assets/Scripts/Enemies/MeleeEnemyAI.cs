using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    protected override void SetupBT()
    {
        attackType = AttackType.Melee;

        rootNode = new BTSelector(new List<BTNode>
        {
            // Player
            new BTSequence(new List<BTNode>
            {
                new BTCondition(() => DistanceToTarget(player) <= detectionRange && currentFocusTime <= 0),

                new BTSelector(new List<BTNode>
                {
                    // Tấn công player
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) <= attackRange),
                        new BTAction(() => RotateToTarget(player)),
                        new BTAction(() => AttackTarget(player))
                    }),

                    // Di chuyển tới player
                    new BTSequence(new List<BTNode>
                    {
                        new BTAction(() => RotateToTarget(player)),
                        new BTAction(() => MoveToTarget(player))
                    })
                })
            }),

            // Tower
            new BTSelector(new List<BTNode>
            {
                // Tấn công tower
                new BTSequence(new List<BTNode>
                {
                    new BTCondition(() => DistanceToTarget(tower) <= attackRange),
                    new BTAction(() => RotateToTarget(tower)),
                    new BTAction(() =>
                    {
                        if (currentFocusTime <= 0f) currentFocusTime = attackCooldown;
                        return AttackTarget(tower);
                    })
                }),

                // Di chuyển tới tower
                new BTSequence(new List<BTNode>
                {
                    new BTAction(() => RotateToTarget(tower)),
                    new BTAction(() => MoveToTarget(tower))
                })
            })
        });
    }

    // Animation event
    public void DealDamageToTarget()
    {
        if (currentTarget == null) return;
        currentTarget.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
    }
}
