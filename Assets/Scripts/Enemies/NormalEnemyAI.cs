using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    private float currentFocusTime;
    private float towerFocusTime = 3f;
    
    protected override void SetupBehaviourTree()
    {
        rootNode = new BTSelector(new List<BTNode>
        {
            new BTSequence(new List<BTNode>
            {
                new BTCondition(() => IsPlayerNearby() && currentFocusTime <= 0),
                new BTSelector(new List<BTNode>
                {
                    new BTSequence(new List<BTNode>
                    {
                        new BTCondition(() => DistanceToTarget(player) <= attackRange),
                        new BTAction(() => AttackTarget(player))
                    }),
                    new BTAction(() => MoveTowards(player))
                })
            }),
            
            new BTSelector(new List<BTNode>
            {
                new BTSequence(new List<BTNode>
                {
                    new BTCondition(() => DistanceToTarget(tower) <= attackRange),
                    new BTAction(() => {
                        currentFocusTime = towerFocusTime;
                        return AttackTarget(tower);
                    })
                }),
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
