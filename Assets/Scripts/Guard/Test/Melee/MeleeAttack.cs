using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 30f;
    public float attackRate = 1f;
    public float attackRange = 1.5f;
    public LayerMask targetLayer;

    [Header("References")]
    public Transform attackPoint;

    private float nextAttackTime;

    public void DoAttack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + 1f / attackRate;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, targetLayer);

        foreach (Collider col in hits)
        {
            IDamageable dmg = col.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawSphere(attackPoint.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
