using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    protected override void SetupBT()
    {
        var nodes = new List<BTNode>();

        var additionalNodes = GetAdditionalBTNodes();
        if (additionalNodes != null)
            nodes.AddRange(additionalNodes);

        // Ưu tiên: Player > Troop > Tower
        nodes.Add(BuildTargetSequence(
            () => player,
            () => DistanceToTarget(player) <= stats.detectionRange && currentFocusTime <= 0,
            false
        ));

        nodes.Add(BuildTargetSequence(
            () => nearestTroop,
            () => nearestTroop != null && DistanceToTarget(nearestTroop) <= stats.detectionRange && currentFocusTime <= 0,
            false
        ));

        nodes.Add(BuildTargetSequence(
            () => tower,
            () => true,
            true
        ));

        rootNode = new BTSelector(nodes);
    }

    protected virtual List<BTNode> GetAdditionalBTNodes()
    {
        return null;
    }

    // Tạo sequence chung cho tất cả target
    private BTSequence BuildTargetSequence(System.Func<Transform> getTarget, System.Func<bool> condition, bool isTower)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => condition() && !IsBlockedByAdditionalCondition()),
            new BTSelector(new List<BTNode>
            {
                BuildAttackSequence(getTarget, isTower),
                BuildMoveSequence(getTarget)
            })
        });
    }

    private BTSequence BuildAttackSequence(System.Func<Transform> getTarget, bool isTower)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(getTarget()) <= stats.attackRange),
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() =>
            {
                if (isTower && currentFocusTime <= 0f)
                    currentFocusTime = stats.attackCooldown;
                return AttackTarget(getTarget());
            })
        });
    }

    private BTSequence BuildMoveSequence(System.Func<Transform> getTarget)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() => MoveToTarget(getTarget()))
        });
    }

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
