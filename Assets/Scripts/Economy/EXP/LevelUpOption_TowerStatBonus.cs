using UnityEngine;
using Son.Economy;

/// <summary>
/// Option: tăng stat cho TOWER (áp dụng cho tất cả TowerRunStats trong scene).
/// </summary>
[CreateAssetMenu(
    fileName = "LevelUpOption_TowerStatBonus",
    menuName = "Son/Economy/Level Up Option/Tower Stat Bonus",
    order = 12)]
public class LevelUpOption_TowerStatBonus : LevelUpOptionConfig
{
    public enum TowerStatType
    {
        AttackDamage,
        Range,
        MaxHP
    }

    [Header("Cấu hình cho Tower")]
    public TowerStatType targetStat = TowerStatType.AttackDamage;

    [Tooltip("AttackDamage: +amount\nRange: +amount\nMaxHP: +amount")]
    public float amount = 5f;

    [Tooltip("Khi tăng MaxHP, có heal full máu không?")]
    public bool healToFullOnMaxHPIncrease = true;

    /// <summary>
    /// Bị gọi từ LevelUpPanel. Không dùng playerStats, mà buff Tower.
    /// </summary>
    public override void ApplyEffect(PlayerRunStats playerStats)
    {
        TowerRunStats[] towers = Object.FindObjectsOfType<TowerRunStats>();
        if (towers == null || towers.Length == 0)
        {
            Debug.LogWarning($"[TowerUpgrade] Không tìm thấy TowerRunStats nào trong scene khi apply {id} - {displayName}");
            return;
        }

        foreach (var tower in towers)
        {
            ApplyToSingleTower(tower);
        }
    }

    public override void ApplyEffect()
    {
        ApplyEffect(null);
    }

    private void ApplyToSingleTower(TowerRunStats tower)
    {
        if (tower == null) return;

        switch (targetStat)
        {
            case TowerStatType.AttackDamage:
                tower.bonusAttackDamage += amount;
                Debug.Log($"[TowerUpgrade] {displayName}: +{amount} Damage cho {tower.name} → Total = {tower.TotalAttackDamage}");
                break;

            case TowerStatType.Range:
                tower.AddRange(amount);
                Debug.Log($"[TowerUpgrade] {displayName}: +{amount} Range cho {tower.name} → Range = {tower.attackRange}");
                break;

            case TowerStatType.MaxHP:
                tower.AddMaxHP(amount, healToFullOnMaxHPIncrease);
                Debug.Log($"[TowerUpgrade] {displayName}: +{amount} MaxHP cho {tower.name} → MaxHP = {tower.maxHP}, HP = {tower.currentHP}");
                break;
        }
    }
}
