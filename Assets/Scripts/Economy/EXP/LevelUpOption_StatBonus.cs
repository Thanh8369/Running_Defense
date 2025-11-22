using UnityEngine;
using Son.Economy;

[CreateAssetMenu(
    fileName = "LevelUpOption_StatBonus",
    menuName = "Son/Economy/Level Up Option/Stat Bonus",
    order = 11)]
public class LevelUpOption_StatBonus : LevelUpOptionConfig
{
    public enum TargetStat
    {
        AttackDamage,       // Cộng thêm damage cố định
        AttackSpeedPercent, // Cộng % attack speed
        MaxHP               // Cộng thêm Max HP
    }

    [Header("Cấu hình Stat Bonus")]
    public TargetStat targetStat = TargetStat.AttackDamage;

    [Tooltip("Giá trị cộng thêm.\n" +
             "- AttackDamage: cộng thẳng vào bonusAttackDamage.\n" +
             "- AttackSpeedPercent: ví dụ 0.1 = +10%.\n" +
             "- MaxHP: cộng thẳng vào MaxHP.")]
    public float amount = 5f;

    [Tooltip("Khi tăng MaxHP, có hồi máu đầy luôn không?")]
    public bool healToFullOnMaxHPIncrease = true;

    /// <summary>
    /// Bản dùng param – gọi từ hệ thống LevelUp, truyền sẵn PlayerRunStats.
    /// </summary>
    public override void ApplyEffect(PlayerRunStats stats)
    {
        if (stats == null)
        {
            Debug.LogError($"[LevelUpOption_StatBonus] stats == null khi apply {id} - {displayName}");
            return;
        }

        switch (targetStat)
        {
            case TargetStat.AttackDamage:
                stats.bonusAttackDamage += amount;
                Debug.Log($"[LevelUpOption_StatBonus] +{amount} Attack Damage. New Damage = {stats.TotalAttackDamage}");
                break;

            case TargetStat.AttackSpeedPercent:
                // amount = 0.1f => +10% attack speed
                float multiplier = 1f + amount;
                stats.attackSpeed *= multiplier;
                Debug.Log($"[LevelUpOption_StatBonus] +{amount * 100f}% Attack Speed. New AttackSpeed = {stats.attackSpeed}");
                break;

            case TargetStat.MaxHP:
                stats.AddMaxHP(amount, healToFullOnMaxHPIncrease);
                Debug.Log($"[LevelUpOption_StatBonus] +{amount} MaxHP. New MaxHP = {stats.maxHP}, CurrentHP = {stats.currentHP}");
                break;
        }
    }

    /// <summary>
    /// Giữ lại bản không param cho tiện nếu bạn đang dùng ở nơi cũ.
    /// Tự tìm PlayerRunStats trong scene.
    /// </summary>
    //public override void ApplyEffect()
    //{
    //    PlayerRunStats stats = Object.FindAnyObjectByType<PlayerRunStats>();
    //    if (stats == null)
    //    {
    //        Debug.LogError($"[LevelUpOption_StatBonus] Không tìm thấy PlayerRunStats trong scene khi apply {id} - {displayName}");
    //        return;
    //    }

    //    ApplyEffect(stats);
    //}
}
