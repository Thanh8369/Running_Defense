using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    [Header("Normal Enemy Settings")]
    public float towerFocusTime = 3f;
    private float currentFocusTime;

    protected override void Start()
    {
        maxHealth = 100f;
        health = maxHealth;
        moveSpeed = 3f;
        attackDamage = 10f;
        attackRange = 2f;
        detectionRange = 8f;
        attackCooldown = 1f;
        currentFocusTime = 0;

        base.Start();
    }

    protected override void SetupBehaviourTree()
    {
        // Behaviour Tree cho Normal Enemy:
        // 1. Nếu đang focus trụ -> giảm timer focus
        // 2. Nếu player gần và không còn focus -> tấn công player
        // 3. Ngược lại -> tấn công trụ

        rootNode = new BTSelector(new List<BTNode>
        {
            // Branch 1: Chase and attack player nếu gần và không focus
            new BTSequence(new List<BTNode>
            {
                new BTCondition(() => IsPlayerNearby() && currentFocusTime <= 0),
                new BTSelector(new List<BTNode>
                {
                    // Nếu đủ gần -> tấn công
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) <= attackRange),
                        new BTAction(() => AttackTarget(player))
                    }),
                    // Không đủ gần -> di chuyển đến
                    new BTAction(() => MoveTowards(player))
                })
            }),
            
            // Branch 2: Focus vào trụ
            new BTSelector(new List<BTNode>
            {
                // Nếu đủ gần trụ -> tấn công
                new BTSequence(new List<BTNode>
                {
                    new BTCondition(() => DistanceToTarget(tower) <= attackRange),
                    new BTAction(() => {
                        currentFocusTime = towerFocusTime;
                        return AttackTarget(tower);
                    })
                }),
                // Không đủ gần -> di chuyển đến trụ
                new BTAction(() => {
                    currentFocusTime -= Time.deltaTime;
                    return MoveTowards(tower);
                })
            })
        });
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
