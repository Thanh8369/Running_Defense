using System.Collections.Generic;
using UnityEngine;

public class WormAI : CyclopsAI
{
    protected override void SetupBT()
    {
        var nodes = new List<BTNode>();

        // Thêm các node tuỳ chỉnh từ class con
        var additionalNodes = GetAdditionalBTNodes();
        if (additionalNodes != null)
            nodes.AddRange(additionalNodes);

        // Ưu tiên: Player > Troop > Tower
        // Chỉ tấn công nếu target trong range, không quay người nếu không trong range
        nodes.Add(BuildStationaryTargetSequence(
            () => player,
            () => DistanceToTarget(player) <= stats.detectionRange && currentFocusTime <= 0
        ));

        nodes.Add(BuildStationaryTargetSequence(
            () => nearestTroop,
            () => nearestTroop != null && DistanceToTarget(nearestTroop) <= stats.detectionRange && currentFocusTime <= 0
        ));

        nodes.Add(BuildStationaryTowerSequence());

        rootNode = new BTSelector(nodes);
    }

    private BTSequence BuildStationaryTargetSequence(System.Func<Transform> getTarget, System.Func<bool> condition)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(condition),
            new BTSelector(new List<BTNode>
            {
                BuildStationaryFireSequence(getTarget)
            })
        });
    }

    private BTSequence BuildStationaryTowerSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTAction(() => BuildStationaryFireSequence(() => tower).Evaluate())
        });
    }

    private BTSequence BuildStationaryFireSequence(System.Func<Transform> getTarget)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(getTarget()) <= stats.attackRange),
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() => AttackTarget(getTarget()))
        });
    }
}
