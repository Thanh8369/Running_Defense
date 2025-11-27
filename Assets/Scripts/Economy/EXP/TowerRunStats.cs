using System;
using UnityEngine;

/// <summary>
/// Chỉ số runtime cho 1 trụ.
/// Các script bắn / AI trụ nên đọc stat từ đây.
/// </summary>
public class TowerRunStats : MonoBehaviour
{
    [Header("ScriptableObject gốc (base stats)")]
    [Tooltip("SO chứa stat cơ bản của trụ.")]
    public TowerData baseData;
    //public Health baseHPData;

    [Header("Damage / Attack (runtime)")]
    [Tooltip("Damage cơ bản của trụ (lấy từ baseData.damage khi init).")]
    public float baseAttackDamage = 10f;

    [Tooltip("Damage cộng thêm từ upgrade / buff.")]
    public float bonusAttackDamage = 0f;

    [Tooltip("Số đòn / giây.")]
    public float attackSpeed = 1f; // hit/second (nếu sau này có trong TowerData thì bổ sung)

    [Header("Range / HP (runtime)")]
    [Tooltip("Tầm bắn của trụ (lấy từ baseData.attackRange khi init).")]
    public float attackRange = 5f;

    [Tooltip("Máu tối đa (lấy từ baseData.maxHealth khi init).")]
    public float maxHP = 100f;

    [Tooltip("Máu hiện tại.")]
    public float currentHP = 100f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;

    /// <summary>
    /// Damage thực tế = base + bonus.
    /// </summary>
    public float TotalAttackDamage => baseAttackDamage + bonusAttackDamage;

    private void Awake()
    {
        InitFromTowerData();
    }

    /// <summary>
    /// Lấy base stat từ TowerData.
    /// Gọi ở Awake hoặc bằng ContextMenu trong Editor.
    /// </summary>
    [ContextMenu("Init From TowerData (Editor Only)")]
    public void InitFromTowerData()
    {
        if (baseData == null)
        {
            Debug.LogWarning($"[TowerRunStats] baseData chưa gán trên {name}, dùng giá trị mặc định.");
            // vẫn giữ các giá trị đang đặt trong Inspector
            currentHP = maxHP;
            return;
        }

        // maxHealth / damage là int, cast sang float cho stats runtime
        maxHP = baseData.maxHealth;
        currentHP = maxHP;

        baseAttackDamage = baseData.damage;
        attackRange = baseData.attackRange;

        // Nếu sau này thêm attackSpeed vào TowerData thì set ở đây luôn.
        // attackSpeed = baseData.attackSpeed;
    }

    /// <summary>
    /// Cộng thêm MaxHP, có thể heal full nếu muốn.
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
    /// Cộng thêm tầm bắn.
    /// </summary>
    public void AddRange(float amount)
    {
        attackRange += amount;
    }
    /// <summary>
    /// Hồi full máu và bắn event OnRevive.
    /// </summary>
    public void ReviveToFull()
    {
        float targetMax = baseData != null ? baseData.maxHealth : maxHP;

        maxHP = targetMax;
        currentHP = targetMax;

        Debug.Log($"[TowerRunStats] ReviveToFull, HP = {currentHP}/{maxHP}");

        OnHealthChanged?.Invoke(currentHP, maxHP);
        OnRevive?.Invoke();
    }
}
