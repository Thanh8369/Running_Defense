using UnityEngine;

/// <summary>
/// Script test để thử hệ thống nhận Gem khi thắng.
/// - Gắn script này vào một GameObject trong Scene gameplay.
/// - Nhấn phím L để "thắng trận".
/// - Sẽ cộng Gem vào Ví + hiện popup.
/// </summary>
public class VictoryTestGemReward : MonoBehaviour
{
    [Header("Test Input")]
    [Tooltip("Phím để test thắng trận.")]
    public KeyCode victoryKey = KeyCode.L;

    [Header("Gem Reward")]
    [Tooltip("Gem thưởng cơ bản.")]
    public int baseGemReward = 100;

    [Tooltip("Gem cộng thêm theo độ khó.")]
    public int bonusPerDifficulty = 20;

    [Tooltip("Độ khó (0 = dễ, 1 = bình thường, 2 = khó...).")]
    public int difficultyLevel = 0;

    private bool _rewardGiven = false;

    private void Update()
    {
        if (Input.GetKeyDown(victoryKey))
        {
            Debug.Log("[VictoryTestGemReward] Nhấn L → Victory()");
            Victory();
        }
    }

    private void Victory()
    {
        if (_rewardGiven)
        {
            Debug.Log("[VictoryTestGemReward] Đã thưởng rồi, không thưởng lại.");
            return;
        }

        // Tính Gem thưởng
        int totalGem = baseGemReward + Mathf.Max(0, difficultyLevel) * bonusPerDifficulty;

        if (WalletManager.Instance == null)
        {
            Debug.LogWarning("[VictoryTestGemReward] WalletManager.Instance == null, không thể cộng Gem.");
            return;
        }

        // 1) Cộng Gem vào ví
        WalletManager.Instance.AddCurrency(CurrencyType.Gem, totalGem, "Victory reward test");

        // 2) Spawn popup (nếu muốn)
        if (GoldPopupSpawner.Instance != null)
        {
            // Spawn ngay tại giữa màn hình (camera center)
            Vector3 worldPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;

            GoldPopupSpawner.Instance.SpawnGoldPopup(worldPos, totalGem);
        }

        Debug.Log($"[VictoryTestGemReward] Victory! Thưởng {totalGem} Gem.");
        _rewardGiven = true;
    }
}
