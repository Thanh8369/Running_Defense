using System;
using UnityEngine;

/// <summary>
/// Chứa các chỉ số runtime của Player trong 1 run.
/// Khi Level Up / buff sẽ cộng vào đây, không sửa ScriptableObject gốc.
/// </summary>
public class PlayerRunStats : MonoBehaviour
{
    [Header("ScriptableObject gốc (base stats)")]
    [Tooltip("SO damage mũi tên (base).")]
    public PlayerArrowDama arrowData;

    [Tooltip("SO damage kiếm (base).")]
    public PlayerSwordDama swordData;       // <--- NEW

    [Tooltip("SO data player, đang dùng shootInterval.")]
    public PlayerData playerData;

    [Tooltip("SO data máu tối đa.")]
    public PlayerHpData hpData;

    [Header("Damage / Attack (runtime)")]
    [Tooltip("Damage cơ bản của mũi tên, lấy từ arrowData.damage khi Start/Awake.")]
    public float baseAttackDamage = 10f;

    [Tooltip("Damage cộng thêm từ các level up, buff... (cho mũi tên).")]
    public float bonusAttackDamage = 0f;

    [Tooltip("Attack per second (số đòn / giây). Sẽ tính từ 1 / shootInterval.")]
    public float attackSpeed = 1f; // 1 = 1 hit/second

    [Header("Sword Damage (runtime)")]
    [Tooltip("Damage cơ bản của kiếm, lấy từ swordData.damage.")]
    public float baseSwordDamage = 10f;      // <--- NEW
    public float baseSwordAttackSpeed;

    [Tooltip("Damage cộng thêm cho kiếm từ level up, buff...")]
    public float bonusSwordDamage = 0f;      // <--- NEW

    [Header("HP (runtime)")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    /// <summary>
    /// Damage thực tế của mũi tên = base + bonus.
    /// </summary>
    public float TotalAttackDamage => baseAttackDamage + bonusAttackDamage;

    /// <summary>
    /// Damage thực tế của kiếm = base + bonus.
    /// </summary>
    public float TotalSwordDamage => baseSwordDamage + bonusSwordDamage;   // <--- NEW

    private void Awake()
    {
        InitFromScriptableData();
    }

    /// <summary>
    /// Lấy base stat từ các ScriptableObject.
    /// Gọi ở Awake hoặc bằng context menu trong Editor.
    /// </summary>
    [ContextMenu("Init From Scriptable Data (Editor Only)")]
    public void InitFromScriptableData()
    {
        // Base damage từ PlayerArrowDama (mũi tên)
        if (arrowData != null)
        {
            baseAttackDamage = arrowData.damage;
        }
        else
        {
            Debug.LogWarning("[PlayerRunStats] arrowData chưa gán, dùng baseAttackDamage mặc định.");
        }

        // Base damage từ PlayerSwordDama (kiếm)
        if (swordData != null)
        {
            baseSwordDamage = swordData.damage;
            baseSwordAttackSpeed = swordData.SwordAttackSpeed;
        }
        else
        {
            Debug.LogWarning("[PlayerRunStats] swordData chưa gán, dùng baseSwordDamage mặc định.");
        }

        // AttackSpeed = 1 / shootInterval
        if (playerData != null)
        {
            if (playerData.shootInterval > 0f)
            {
                attackSpeed = 1f / playerData.shootInterval;
            }
            else
            {
                Debug.LogWarning("[PlayerRunStats] playerData.shootInterval <= 0, giữ nguyên attackSpeed.");
            }
        }
        else
        {
            Debug.LogWarning("[PlayerRunStats] playerData chưa gán, dùng attackSpeed mặc định.");
        }

        // HP từ PlayerHpData
        if (hpData != null)
        {
            maxHP = hpData._maxHealth;
            currentHP = maxHP;
        }
        else
        {
            Debug.LogWarning("[PlayerRunStats] hpData chưa gán, dùng maxHP mặc định.");
            currentHP = maxHP;
        }
    }

    /// <summary>
    /// Cộng thêm MaxHP, đồng thời có thể hồi máu full nếu muốn.
    /// </summary>
    public void AddMaxHP(float amount, bool healToFull = false)
    {
        maxHP += amount;
        if (healToFull)
        {
            currentHP = maxHP;
        }
        else
        {
            currentHP = Mathf.Min(currentHP, maxHP);
        }
    }

    /// <summary>
    /// Level up riêng cho kiếm: cộng thêm sword damage.
    /// </summary>
    public void AddSwordDamage(float amount)       // <--- NEW, dùng cho level up
    {
        bonusSwordDamage += amount;
    }

    /// <summary>
    /// Tăng % tốc độ tấn công của kiếm (quay quanh player).
    /// amount = 0.2f => +20%.
    /// </summary>
    public void AddSwordAttackSpeedPercent(float amount)
    {
        float mul = 1f + amount;
        baseSwordAttackSpeed *= mul;
    }

    /// <summary>
    /// Tăng % tốc độ tấn công của mũi tên.
    /// amount = 0.2f => +20%.
    /// </summary>
    public void AddAttackSpeedPercent(float amount)
    {
        float mul = 1f + amount;
        attackSpeed *= mul;
    }
}
