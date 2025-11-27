using System.Collections.Generic;
using UnityEngine;

public class EvilMageAI : RangedEnemyAI
{
    [SerializeField] private List<AnimationFirepointProjectileMap> attackMappings = new List<AnimationFirepointProjectileMap>();

    private AnimationFirepointProjectileMap currentAttackMapping;
    private AnimationFirepointProjectileMap mappingUsedForCurrentProjectile;

    private GameObject currentProjectile;
    private float projectileScaleStartTime;

    private readonly Vector3 defaultScale = new Vector3(0.3f, 0.3f, 0.3f);   // projectile loại 1 mặc định

    public void OnAttackAnimationSet(string triggerName)
    {
        currentAttackMapping = GetMappingForAnimation(triggerName);
        if (currentAttackMapping != null)
            SetProjectileVariant(currentAttackMapping.projectileVariantIndex);
    }

    private AnimationFirepointProjectileMap GetMappingForAnimation(string triggerName)
    {
        foreach (var map in attackMappings)
        {
            if (map.animationTriggerName == triggerName && map.firePoint != null)
                return map;
        }
        return null;
    }

    public override void FireProjectile()
    {
        // Reset state TRƯỚC KHI tạo projectile mới
        currentProjectile = null;
        mappingUsedForCurrentProjectile = null;
        projectileScaleStartTime = 0f;

        if (currentTarget == null || currentAttackMapping == null) return;

        var projectileVariants = stats.projectilePrefabs;
        if (projectileVariants.Count == 0) return;

        int variantIndex = Mathf.Clamp(currentAttackMapping.projectileVariantIndex, 0, projectileVariants.Count - 1);
        ProjectileData projData = projectileVariants[variantIndex];

        if (projData.prefab == null) return;

        // Spawn projectile
        currentProjectile = PoolManager.Instance.Get(
            projData.prefab, 
            currentAttackMapping.firePoint.position,
            currentAttackMapping.firePoint.rotation
        );

        EnemyProjectile p = currentProjectile.GetComponent<EnemyProjectile>();

        if (p != null)
        {
            float finalDamage = projData.customDamage > 0 ? projData.customDamage : stats.attackDamage;
            p.Initialize(currentTarget, finalDamage, currentAttackMapping.firePoint.forward);
            p.StopMoving();
        }

        // Ghi lại mapping dùng cho projectile này
        mappingUsedForCurrentProjectile = currentAttackMapping;

        // Nếu attack này có scale-up => bắt đầu từ scale 0
        if (currentAttackMapping.scaleUpDuration > 0)
        {
            currentProjectile.transform.localScale = Vector3.zero;
            projectileScaleStartTime = Time.time;
        }
        else
        {
            // Ngược lại: scale mặc định cho loại 1
            currentProjectile.transform.localScale = defaultScale;
        }
    }

    public void LaunchCurrentProjectile()
    {
        if (currentProjectile == null) return;
        EnemyProjectile p = currentProjectile.GetComponent<EnemyProjectile>();
        p?.StartMoving();
    }

    protected override void Update()
    {
        base.Update();

        // Scale-up chỉ áp dụng nếu:
        // 1. Có projectile
        // 2. Projectile đang active
        // 3. mappingUsedForCurrentProjectile != null
        // 4. mappingUsedForCurrentProjectile có scaleUpDuration > 0
        if (currentProjectile != null && 
            currentProjectile.activeInHierarchy &&
            mappingUsedForCurrentProjectile != null &&
            mappingUsedForCurrentProjectile.scaleUpDuration > 0)
        {
            float elapsed = Time.time - projectileScaleStartTime;
            float progress = Mathf.Clamp01(elapsed / mappingUsedForCurrentProjectile.scaleUpDuration);

            currentProjectile.transform.localScale =
                Vector3.one * (progress * mappingUsedForCurrentProjectile.scaleUpMax);
        }
    }
}
