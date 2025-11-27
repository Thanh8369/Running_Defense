using System.Collections;
using UnityEngine;

public class DemonKingAI : MeleeEnemyAI
{
    [Header("Slow Zone Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject slowZonePrefab;
    [SerializeField] private float slowZoneLifetime = 5f;
    [SerializeField] private SlowDebuffConfig slowDebuffConfig;

    [Header("Summon Settings")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private GameObject summonCirclePrefab;
    [SerializeField] private int summonRadius;

    private string currentAttackVariantTrigger = "";

    private new void OnEnable()
    {
        FindFirstObjectByType<BossHealthUI>(FindObjectsInactive.Include)
            ?.HandleBossSpawned(this);
    }

    public void OnAttackAnimationSet(string triggerName)
    {
        if (stats.attackVariants.Exists(v => v.triggerName == triggerName))
            currentAttackVariantTrigger = triggerName;
    }

    public new void DealDamageToTarget()
    {
        if (currentTarget == null) return;

        var variant = GetCurrentAttackVariant();
        float damage = variant != null ? variant.damageAmount : stats.attackDamage;

        currentTarget.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    /// <summary>
    /// Gọi từ Animation Event
    /// </summary>
    public void CreateSlowZone()
    {
        if (slowZonePrefab == null || slowDebuffConfig == null)
        {
            Debug.LogError("SlowZone prefab hoặc SlowDebuffConfig chưa assign!");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject zoneObj = PoolManager.Instance.Get(
            slowZonePrefab,
            spawnPos,
            Quaternion.identity
        );

        SlowZone slowZone = zoneObj.GetComponent<SlowZone>();
        if (slowZone != null)
        {
            slowZone.Initialize(slowDebuffConfig, slowZoneLifetime);
        }
    }

    public void SummonMinions()
    {
        if (minionPrefab == null || summonCirclePrefab == null) return;

        Vector2 offset = Random.insideUnitCircle * summonRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, 0, offset.y);

        GameObject circle = PoolManager.Instance.Get(
            summonCirclePrefab,
            spawnPos,
            Quaternion.identity
        );

        circle.GetComponent<SummonCircle>().Initialize(minionPrefab, spawnPos);
    }

    private AttackVariant GetCurrentAttackVariant()
    {
        foreach (var variant in stats.attackVariants)
        {
            if (variant.triggerName == currentAttackVariantTrigger)
                return variant;
        }
        return null;
    }
}
