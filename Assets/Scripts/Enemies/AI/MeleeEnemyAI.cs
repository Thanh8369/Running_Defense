using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    protected override void SetupBT()
    {
        var nodes = new List<BTNode>
        {
            BuildPlayerAttackSequence(),
            BuildTowerAttackSequence()
        };

        // Thêm các node tuỳ chỉnh từ class con
        var additionalNodes = GetAdditionalBTNodes();
        if (additionalNodes != null)
            nodes.InsertRange(0, additionalNodes);

        rootNode = new BTSelector(nodes);
    }

    // Override này để thêm behavior tuỳ chỉnh (ví dụ: heal, buff, etc)
    protected virtual List<BTNode> GetAdditionalBTNodes()
    {
        return null;
    }

    private BTSequence BuildPlayerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => 
                DistanceToTarget(player) <= stats.detectionRange 
                && currentFocusTime <= 0 
                && !IsBlockedByAdditionalCondition()),
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
            new BTCondition(() => !IsBlockedByAdditionalCondition()),
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

    // Override để chặn hành động khi cần (không nên bao gồm isHit ở đây)
    protected virtual bool IsBlockedByAdditionalCondition()
    {
        return false;
    }

    public virtual void DealDamageToTarget()
    {
        if (currentTarget == null) return;
        currentTarget.GetComponent<IDamageable>()?.TakeDamage(stats.attackDamage);
    }
}
