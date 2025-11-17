using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Son.Economy;

/// <summary>
/// Dùng cho cả Weekly / Monthly login reward.
/// Gắn script này vào root của mỗi panel Reward.
/// </summary>
public class StreakRewardUI : MonoBehaviour
{
    [Header("Config chung")]
    [Tooltip("ID duy nhất để lưu PlayerPrefs,ví dụ: WEEK_REWARD, MONTH_REWARD")]
    public string rewardId = "WEEK_REWARD";

    [Tooltip("Tổng số ngày trong 1 vòng: 7 cho week, 30 cho month")]
    public int totalDays = 7;

    [Tooltip("Root panel để bật/tắt")]
    public GameObject panelRoot;

    [Header("UI header")]
    public TextMeshProUGUI titleText;   // ví dụ: "Daily Login Rewards 2/7"

    [Header("Button Claim")]
    public Button claimButton;

    [Header("Các ô ngày (sắp xếp đúng thứ tự trong Inspector)")]
    public StreakRewardDayUI[] daySlots;

    [Header("Cấu hình thưởng từng ngày")]
    public RewardEntry[] rewardEntries; // cùng size với totalDays

    private int _currentDayIndex; // 0..totalDays-1
    private DateTime _lastClaimDate;

    private string PrefDayKey => $"{rewardId}_DAY_INDEX";
    private string PrefDateKey => $"{rewardId}_LAST_DATE";

    private void Awake()
    {
        LoadProgress();
        RefreshUI();
    }

    /// <summary>
    /// Kiểm tra có thể nhận thưởng hôm nay không.
    /// </summary>
    public bool CanClaimToday()
    {
        // chưa claim bao giờ → được claim
        if (_lastClaimDate == DateTime.MinValue) return true;

        var today = DateTime.UtcNow.Date;
        return today > _lastClaimDate; // khác ngày thì được claim
    }

    /// <summary>
    /// Hàm bấm nút Claim (set trong OnClick của button).
    /// </summary>
    public void OnClickClaim()
    {
        if (!CanClaimToday())
        {
            Debug.Log($"[{rewardId}] Hôm nay đã nhận rồi.");
            return;
        }

        // Lấy thông tin reward của ngày hiện tại
        RewardEntry entry = rewardEntries[_currentDayIndex];

        // Add vào Wallet: hiện tại chỉ hỗ trợ Gold (Coin) và Gem
        switch (entry.type)
        {
            case RewardType.Gold: // dùng Gold trong Wallet
                WalletManager.Instance.AddCurrency(CurrencyType.Gold, entry.amount, $"DailyLogin_{rewardId}");
                break;

            case RewardType.Gem:
                WalletManager.Instance.AddCurrency(CurrencyType.Gem, entry.amount, $"DailyLogin_{rewardId}");
                break;

            // Các loại khác tạm thời chưa có hệ thống riêng → chỉ log cảnh báo
            default:
                Debug.LogWarning(
                    $"[{rewardId}] Reward type {entry.type} chưa được hỗ trợ (chỉ Gold & Gem). " +
                    $"Reward vẫn được đánh dấu là đã claim nhưng không cộng vào Wallet."
                );
                break;
        }

        // Cập nhật ngày + ngày claim
        _lastClaimDate = DateTime.UtcNow.Date;
        _currentDayIndex++;

        if (_currentDayIndex >= totalDays)
        {
            // hoàn tất 1 vòng → quay lại ngày 0
            _currentDayIndex = 0;
        }

        SaveProgress();
        RefreshUI();
    }

    private void LoadProgress()
    {
        _currentDayIndex = PlayerPrefs.GetInt(PrefDayKey, 0);

        string dateStr = PlayerPrefs.GetString(PrefDateKey, "");
        if (string.IsNullOrEmpty(dateStr))
        {
            _lastClaimDate = DateTime.MinValue;
        }
        else
        {
            long binary = Convert.ToInt64(dateStr);
            _lastClaimDate = DateTime.FromBinary(binary);
        }

        // clamp cho chắc
        if (_currentDayIndex < 0 || _currentDayIndex >= totalDays)
            _currentDayIndex = 0;
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(PrefDayKey, _currentDayIndex);

        long binary = _lastClaimDate.ToBinary();
        PlayerPrefs.SetString(PrefDateKey, binary.ToString());

        PlayerPrefs.Save();
    }

    private void RefreshUI()
    {
        // tiêu đề kiểu "Daily Login Rewards 2/7"
        if (titleText != null)
        {
            int displayDay = _currentDayIndex + 1;
            titleText.text = $"Daily Login Rewards  {displayDay}/{totalDays}";
        }

        bool canClaim = CanClaimToday();
        claimButton.interactable = canClaim;

        // setup từng ô
        for (int i = 0; i < daySlots.Length; i++)
        {
            var slot = daySlots[i];
            RewardEntry entry = rewardEntries[i];

            // icon + số lượng
            slot.iconImage.sprite = entry.icon;
            slot.amountText.text = entry.amount.ToString();

            // ô đã qua / hôm nay / tương lai
            if (i < _currentDayIndex)
            {
                slot.SetStateClaimed();
            }
            else if (i == _currentDayIndex)
            {
                slot.SetStateToday(canClaim);
            }
            else
            {
                slot.SetStateLocked();
            }
        }
    }

    /// <summary>
    /// Gọi hàm này khi bạn muốn mở panel (vd từ script điều khiển sequence).
    /// </summary>
    public void Show()
    {
        panelRoot.SetActive(true);
        RefreshUI();
    }

    /// <summary>
    /// Ẩn panel sau khi claim xong.
    /// </summary>
    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}

[Serializable]
public class RewardEntry
{
    public RewardType type;
    public int amount;
    public Sprite icon;
}

/// <summary>
/// Script gắn vào từng ô Day (Day 1 / Day 2 / ...).
/// </summary>
[Serializable]
public class StreakRewardDayUI
{
    public Image iconImage;
    public TextMeshProUGUI amountText;
    public GameObject claimedTick;
    public GameObject todayHighlight;

    public void SetStateClaimed()
    {
        claimedTick.SetActive(true);
        todayHighlight.SetActive(false);
    }

    public void SetStateToday(bool canClaim)
    {
        claimedTick.SetActive(false);
        todayHighlight.SetActive(true);
    }

    public void SetStateLocked()
    {
        claimedTick.SetActive(false);
        todayHighlight.SetActive(false);
    }
}
