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
                BuildFireSequence(player),
                BuildAdvanceSequence(player)
            })
        });
    }

    private BTSequence BuildTowerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTSelector(new List<BTNode>
            {
                BuildTowerFireWithCooldown(),
                BuildAdvanceSequence(tower)
            })
        });
    }

    private BTSequence BuildFireSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(target) <= stats.attackRange),
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => AttackTarget(target))
        });
    }

    private BTSequence BuildAdvanceSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(target) > stats.attackRange),
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => MoveToTarget(target))
        });
    }

    private BTSequence BuildTowerFireWithCooldown()
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
