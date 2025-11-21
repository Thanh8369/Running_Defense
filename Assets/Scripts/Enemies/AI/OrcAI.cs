using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrcAttackVariant
{
    public int id;
    public bool isAOE = false;
    public float aoeDamage = 15f;
}

public class OrcAI : MeleeEnemyAI
{
    [SerializeField] private List<OrcAttackVariant> attackVariants = new List<OrcAttackVariant>();
    [SerializeField] private Transform aoeEffectPoint;

    private int currentAttackVariantId = 0;

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

    public void OnAttackAnimationSet(string triggerName)
    {
        if (triggerName.Contains("Attack02"))
            currentAttackVariantId = 1;
        else
            currentAttackVariantId = 0;
    }

    public new void DealDamageToTarget()
    {
        if (currentTarget == null) return;

        OrcAttackVariant currentVariant = GetCurrentAttackVariant();

        if (currentVariant != null && currentVariant.isAOE)
        {
            DealAOEDamage(currentVariant);
        }
        else
        {
            DealSingleTargetDamage();
        }
    }

    private OrcAttackVariant GetCurrentAttackVariant()
    {
        foreach (var variant in attackVariants)
        {
            if (variant.id == currentAttackVariantId)
                return variant;
        }
        return null;
    }

    private void DealSingleTargetDamage()
    {
        currentTarget.GetComponent<IDamageable>()?.TakeDamage(stats.attackDamage);
    }

    private void DealAOEDamage(OrcAttackVariant variant)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stats.attackRange);

        foreach (Collider collider in hitColliders)
        {
            if (collider.transform == transform) continue; // Skip tự mình

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(variant.aoeDamage);
            }
        }
    }
}
