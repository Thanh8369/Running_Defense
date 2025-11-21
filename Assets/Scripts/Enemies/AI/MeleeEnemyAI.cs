using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    protected override void SetupBT()
    {
        attackType = AttackType.Melee;
        rootNode = new BTSelector(new List<BTNode>
        {
            BuildPlayerAttackSequence(),
            BuildTowerAttackSequence()
        });
    }

    private BTSequence BuildPlayerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(player) <= stats.detectionRange && currentFocusTime <= 0),
            new BTSelector(new List<BTNode>
            {
                BuildAttackSequence(player),
                BuildMoveSequence(player)
            })
        });
    }

    private BTSequence BuildTowerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTSelector(new List<BTNode>
            {
                BuildTowerAttackWithCooldown(),
                BuildMoveSequence(tower)
            })
        });
    }

    private BTSequence BuildAttackSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(target) <= stats.attackRange),
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => AttackTarget(target))
        });
    }

    private BTSequence BuildMoveSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => MoveToTarget(target))
        });
    }

    private BTSequence BuildTowerAttackWithCooldown()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(tower) <= stats.attackRange),
            new BTAction(() => RotateToTarget(tower)),
            new BTAction(() =>
            {
                if (currentFocusTime <= 0f) currentFocusTime = stats.attackCooldown;
                return AttackTarget(tower);
            })
        });
    }

    public void DealDamageToTarget()
    {
        if (currentTarget == null) return;
        currentTarget.GetComponent<IDamageable>()?.TakeDamage(stats.attackDamage);
    }
}
