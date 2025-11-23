using UnityEngine;
using Son.Economy;

/// <summary>
/// Option: tăng stat cho PLAYER (damage, attack speed, HP).
/// </summary>
[CreateAssetMenu(
    fileName = "LevelUpOption_StatBonus",
    menuName = "Son/Economy/Level Up Option/Player Stat Bonus",
    order = 11)]
public class LevelUpOption_StatBonus : LevelUpOptionConfig
{
    public enum PlayerStatType
    {
        AttackDamage,       // Cộng thẳng vào bonus damage
        AttackSpeedPercent, // Nhân % attack speed
        MaxHP               // Tăng Max HP
    }

    [Header("Cấu hình cho Player")]
    public PlayerStatType targetStat = PlayerStatType.AttackDamage;

    [Tooltip("AttackDamage: +amount\nAttackSpeedPercent: amount = 0.1f => +10%\nMaxHP: +amount")]
    public float amount = 5f;

    [Tooltip("Khi tăng MaxHP, có heal full máu không?")]
    public bool healToFullOnMaxHPIncrease = true;

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
                Debug.Log($"[PlayerUpgrade] {displayName}: +{amount} Damage → Total = {stats.TotalAttackDamage}");
                break;

            case PlayerStatType.AttackSpeedPercent:
                {
                    float mul = 1f + amount; // amount = 0.1 -> +10%
                    stats.attackSpeed *= mul;
                    Debug.Log($"[PlayerUpgrade] {displayName}: +{amount * 100f}% Attack Speed → AS = {stats.attackSpeed}");
                    break;
                }

            case PlayerStatType.MaxHP:
                stats.AddMaxHP(amount, healToFullOnMaxHPIncrease);
                Debug.Log($"[PlayerUpgrade] {displayName}: +{amount} MaxHP → MaxHP = {stats.maxHP}, HP = {stats.currentHP}");
                break;
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
