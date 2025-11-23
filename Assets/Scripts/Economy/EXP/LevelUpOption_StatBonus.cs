using UnityEngine;
using Son.Economy;

/// <summary>
/// Option: tăng stat cho PLAYER (damage, attack speed, HP, sword).
/// </summary>
[CreateAssetMenu(
    fileName = "LevelUpOption_StatBonus",
    menuName = "Son/Economy/Level Up Option/Player Stat Bonus",
    order = 11)]
public class LevelUpOption_StatBonus : LevelUpOptionConfig
{
    public enum PlayerStatType
    {
        AttackDamage,           // Cộng thẳng vào bonus damage (arrow)
        AttackSpeedPercent,     // Nhân % attack speed (arrow)
        MaxHP,                  // Tăng Max HP
        SwordDamageAndSpeed     // NEW: Kiếm - tăng damage + attack speed trong 1 option
    }

    [Header("Cấu hình chung cho Player")]
    public PlayerStatType targetStat = PlayerStatType.AttackDamage;

    [Tooltip("AttackDamage: +amount\nAttackSpeedPercent: amount = 0.1f => +10%\nMaxHP: +amount")]
    public float amount = 5f;

    [Tooltip("Khi tăng MaxHP, có heal full máu không?")]
    public bool healToFullOnMaxHPIncrease = true;

    [Header("Cấu hình riêng cho Sword (nếu targetStat = SwordDamageAndSpeed)")]
    [Tooltip("Cộng thêm damage cho kiếm (bonus). VD: 10 = +10 damage kiếm.")]
    public float swordDamageAmount = 10f;

    [Tooltip("Tăng % tốc độ quay kiếm. VD: 0.2 = +20% speed.")]
    public float swordAttackSpeedPercent = 0.2f;

    /// <summary>
    /// Gọi từ LevelUpPanel, đã truyền sẵn PlayerRunStats.
    /// </summary>
    public override void ApplyEffect(PlayerRunStats stats)
    {
        if (stats == null)
        {
            Debug.LogError($"[LevelUpOption_StatBonus] PlayerRunStats == null khi apply {id} - {displayName}");
            return;
        }

        switch (targetStat)
        {
            case PlayerStatType.AttackDamage:
                stats.bonusAttackDamage += amount;
                Debug.Log($"[PlayerUpgrade] {displayName}: +{amount} Arrow Damage → Total = {stats.TotalAttackDamage}");
                break;

            case PlayerStatType.AttackSpeedPercent:
                {
                    float mul = 1f + amount; // amount = 0.1 -> +10%
                    stats.attackSpeed *= mul;
                    Debug.Log($"[PlayerUpgrade] {displayName}: +{amount * 100f}% Arrow Attack Speed → AS = {stats.attackSpeed}");
                    break;
                }

            case PlayerStatType.MaxHP:
                stats.AddMaxHP(amount, healToFullOnMaxHPIncrease);
                Debug.Log($"[PlayerUpgrade] {displayName}: +{amount} MaxHP → MaxHP = {stats.maxHP}, HP = {stats.currentHP}");
                break;

            case PlayerStatType.SwordDamageAndSpeed:
                {
                    stats.AddSwordDamage(swordDamageAmount);
                    stats.AddSwordAttackSpeedPercent(swordAttackSpeedPercent);

                    Debug.Log(
                        $"[PlayerUpgrade] {displayName}: "
                        + $"+{swordDamageAmount} Sword Damage (TotalSwordDamage = {stats.TotalSwordDamage}), "
                        + $"+{swordAttackSpeedPercent * 100f}% Sword Attack Speed (baseSwordAttackSpeed = {stats.baseSwordAttackSpeed})"
                    );
                    break;
                }
        }
    }

    /// <summary>
    /// Cho phép test nhanh trong Editor (context menu, v.v.).
    /// </summary>
    public override void ApplyEffect()
    {
        var stats = Object.FindAnyObjectByType<PlayerRunStats>();
        ApplyEffect(stats);
    }
}
