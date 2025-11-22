using UnityEngine;

/// <summary>
/// Chứa các chỉ số runtime của Player trong 1 run.
/// Khi Level Up sẽ cộng vào đây.
/// </summary>
public class PlayerRunStats : MonoBehaviour
{
    [Header("ScriptableObject gốc (base stats)")]
    [Tooltip("SO damage mũi tên (base).")]
    public PlayerArrowDama arrowData;

    [Tooltip("SO data player, đang dùng shootInterval.")]
    public PlayerData playerData;

    [Tooltip("SO data máu tối đa.")]
    public PlayerHpData hpData;

    [Header("Damage / Attack (runtime)")]
    [Tooltip("Damage cơ bản, lấy từ arrowData.damage khi Start.")]
    public float baseAttackDamage = 10f;

    [Tooltip("Damage cộng thêm từ các level up, buff...")]
    public float bonusAttackDamage = 0f;

    [Tooltip("Attack per second (số đòn / giây). Sẽ tính từ 1 / shootInterval.")]
    public float attackSpeed = 1f; // 1 = 1 hit/second

    [Header("HP (runtime)")]
    public float maxHP = 100f;
    public float currentHP = 100f;

    /// <summary>
    /// Damage thực tế = base + bonus (tuỳ anh dùng).
    /// </summary>
    public float TotalAttackDamage => baseAttackDamage + bonusAttackDamage;

    private void Awake()
    {
        InitFromScriptableData();
    }

    /// <summary>
    /// Lấy base stat từ các ScriptableObject.
    /// Gọi ở Awake hoặc context menu trong Editor.
    /// </summary>
    [ContextMenu("Init From Scriptable Data (Editor Only)")]
    public void InitFromScriptableData()
    {
        // Base damage từ PlayerArrowDama
        if (arrowData != null)
        {
            baseAttackDamage = arrowData.damage;
        }
        else
        {
            Debug.LogWarning("[PlayerRunStats] arrowData chưa gán, dùng baseAttackDamage mặc định.");
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
    }
}
