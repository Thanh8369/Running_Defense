using UnityEngine;

/// <summary>
/// Quản lý thưởng Gem khi người chơi thắng trận (Victory).
/// Gắn script này lên 1 GameObject trong scene gameplay
/// hoặc vào GameManager/RunManager hiện tại.
/// 
/// Gọi GrantVictoryReward() khi điều kiện thắng được thỏa.
/// </summary>
public class VictoryGemReward : MonoBehaviour
{
    [Header("Cấu hình thưởng Gem cơ bản")]
    [Tooltip("Số Gem thưởng cơ bản mỗi lần thắng.")]
    public int baseGemReward = 100;

    [Tooltip("Gem cộng thêm theo độ khó (vd: difficultyLevel * bonusPerDifficulty).")]
    public int bonusPerDifficulty = 25;

    [Tooltip("Độ khó hiện tại (0 = dễ, 1 = trung bình, 2 = khó, v.v.).")]
    public int difficultyLevel = 0;

    [Tooltip("Chỉ cho phép thưởng 1 lần mỗi run.")]
    public bool preventDoubleReward = true;

    private bool _hasRewarded = false;

    /// <summary>
    /// Gọi hàm này khi người chơi thắng trận (VD: boss chết, time-out wave cuối, thành không vỡ, v.v.).
    /// Có thể gọi từ GameManager/RunManager hiện tại.
    /// </summary>
    public void GrantVictoryReward()
    {
        if (preventDoubleReward && _hasRewarded)
        {
            Debug.Log("[VictoryGemReward] Đã thưởng Gem rồi, bỏ qua lần gọi thứ 2.");
            return;
        }

        if (WalletManager.Instance == null)
        {
            Debug.LogError("[VictoryGemReward] WalletManager.Instance == null. Không thể cộng Gem.");
            return;
        }

        int totalGem = CalculateGemReward();
        if (totalGem <= 0)
        {
            Debug.LogWarning("[VictoryGemReward] totalGem <= 0, không thưởng.");
            return;
        }

        WalletManager.Instance.AddCurrency(CurrencyType.Gem, totalGem, "Victory Reward");
        _hasRewarded = true;

        Debug.Log($"[VictoryGemReward] Thưởng Victory: {totalGem} Gem (difficultyLevel={difficultyLevel}).");
    }

    /// <summary>
    /// Công thức tính Gem thưởng.
    /// Bạn có thể chỉnh sửa cho phù hợp với balancing của game.
    /// </summary>
    private int CalculateGemReward()
    {
        int bonus = Mathf.Max(0, difficultyLevel) * Mathf.Max(0, bonusPerDifficulty);
        int total = baseGemReward + bonus;
        return Mathf.Max(0, total);
    }
}
