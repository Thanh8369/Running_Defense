using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationFirepointProjectileMap
{
    public string animationTriggerName;
    public Transform firePoint;
    public int projectileVariantIndex = 0;
    public float scaleUpDuration = 0f;
    public float scaleUpMax = 1f;
}

public class RangedEnemyAI : EnemyAI
{
    [SerializeField] protected Transform firePoint;
    protected ProjectileData currentProjectileData;

    protected override void Start()
    {
        base.Start();
        if (stats.projectilePrefabs.Count > 0)
            currentProjectileData = stats.projectilePrefabs[0];
    }

    protected override void SetupBT()
    {
        var nodes = new List<BTNode>();

        // Thêm các node tuỳ chỉnh từ class con
        var additionalNodes = GetAdditionalBTNodes();
        if (additionalNodes != null)
            nodes.AddRange(additionalNodes);

        // Ưu tiên: Player > Troop > Tower
        nodes.Add(BuildTargetSequence(
            () => player,
            () => DistanceToTarget(player) <= stats.detectionRange
        ));

        nodes.Add(BuildTargetSequence(
            () => nearestTroop,
            () => nearestTroop != null && DistanceToTarget(nearestTroop) <= stats.detectionRange
        ));

        nodes.Add(BuildTowerSequence());

        rootNode = new BTSelector(nodes);
    }

    protected virtual List<BTNode> GetAdditionalBTNodes()
    {
        return null;
    }

    private BTSequence BuildTargetSequence(System.Func<Transform> getTarget, System.Func<bool> condition)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(condition),
            new BTSelector(new List<BTNode>
            {
                BuildFireSequence(getTarget),
                BuildAdvanceSequence(getTarget)
            })
        });
    }

    private BTSequence BuildTowerSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTSelector(new List<BTNode>
            {
                BuildTowerFireWithCooldown(),
                BuildAdvanceSequence(() => tower)
            })
        });
    }

    private BTSequence BuildFireSequence(System.Func<Transform> getTarget)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(getTarget()) <= stats.attackRange),
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() => AttackTarget(getTarget()))
        });
    }

    private BTSequence BuildAdvanceSequence(System.Func<Transform> getTarget)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(getTarget()) > stats.attackRange),
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() => MoveToTarget(getTarget()))
        });
    }

    private BTSequence BuildTowerFireWithCooldown()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(tower) <= stats.attackRange),
            new BTAction(() => RotateToTarget(tower)),
            new BTAction(() => AttackTarget(tower))
        });
    }

    public virtual void FireProjectile()
    {
        if (currentTarget == null || firePoint == null) return;
        if (currentProjectileData?.prefab == null) return;

        GameObject proj = PoolManager.Instance.Get(currentProjectileData.prefab, firePoint.position, firePoint.rotation);
        EnemyProjectile p = proj.GetComponent<EnemyProjectile>();

        if (p != null)
        {
            float finalDamage = currentProjectileData.customDamage > 0 ? currentProjectileData.customDamage : stats.attackDamage;
            p.Initialize(currentTarget, finalDamage, firePoint.right);
        }
    }

    public void SetProjectileVariant(int index)
    {
        if (index >= 0 && index < stats.projectilePrefabs.Count)
            currentProjectileData = stats.projectilePrefabs[index];
    }
}
