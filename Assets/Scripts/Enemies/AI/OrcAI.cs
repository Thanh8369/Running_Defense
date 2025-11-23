using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrcAttackVariant
{
    public int id;
    public bool isAOE = false;
    public float aoeDamage = 15f;
    public LayerMask aoeLayerMask;
}

public class OrcAI : MeleeEnemyAI
{
    [SerializeField] private List<OrcAttackVariant> attackVariants = new List<OrcAttackVariant>();
    private int currentAttackVariantId = 0;

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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stats.attackRange, variant.aoeLayerMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.transform == transform) continue;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(variant.aoeDamage);
            }
        }
    }
}
