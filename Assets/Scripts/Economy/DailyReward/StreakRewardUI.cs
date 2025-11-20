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
    [Tooltip("ID duy nhất để lưu PlayerPrefs, ví dụ: WEEK_REWARD, MONTH_REWARD")]
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

        // Add vào Wallet: hiện tại chỉ hỗ trợ Gold và Gem
        switch (entry.type)
        {
            case RewardType.Gold:
                WalletManager.Instance.AddCurrency(CurrencyType.Gold, entry.amount, $"DailyLogin_{rewardId}");
                break;

            case RewardType.Gem:
                WalletManager.Instance.AddCurrency(CurrencyType.Gem, entry.amount, $"DailyLogin_{rewardId}");
                break;

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
            var entry = rewardEntries[i];

            // set icon + value + day text
            slot.SetupVisual(entry, i);

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
    public RewardType type;       // Gold / Gem
    public int amount;            // số lượng
    public Sprite icon;           // icon hiển thị
    [Tooltip("Nếu để trống sẽ tự set = \"Day X\"")]
    public string customDayLabel; // optional
}

/// <summary>
/// Script gắn vào từng ô Day (Day 1 / Day 2 / ...).
/// Mỗi ô gồm:
/// - 2 background: normal & claimed
/// - item icon
/// - check image
/// - text day, text value
/// - focus highlight (today)
/// </summary>
[Serializable]
public class StreakRewardDayUI
{
    [Header("Background state")]
    public Image rewardNormalImage;    // nền khi chưa nhận
    public Image rewardClaimedImage;   // nền khi đã nhận

    [Header("Icon + check")]
    public Image itemIconImage;        // icon quà
    public Image checkImage;           // dấu check đã nhận

    [Header("Text")]
    public TextMeshProUGUI textDay;    // "Day 1"
    public TextMeshProUGUI textValue;  // "100"

    [Header("Highlight hôm nay")]
    public GameObject focusHighlight;  // object Focus

    /// <summary>
    /// Set icon, value, day text cho ô.
    /// </summary>
    public void SetupVisual(RewardEntry entry, int dayIndex)
    {
        if (itemIconImage != null && entry.icon != null)
            itemIconImage.sprite = entry.icon;

        if (textValue != null)
            textValue.text = entry.amount.ToString();

        if (textDay != null)
        {
            if (!string.IsNullOrEmpty(entry.customDayLabel))
                textDay.text = entry.customDayLabel;
            else
                textDay.text = $"Day {dayIndex + 1}";
        }
    }

    public void SetStateClaimed()
    {
        if (rewardNormalImage != null) rewardNormalImage.enabled = false;
        if (rewardClaimedImage != null) rewardClaimedImage.enabled = true;

        if (checkImage != null) checkImage.gameObject.SetActive(true);
        if (focusHighlight != null) focusHighlight.SetActive(false);
    }

    public void SetStateToday(bool canClaim)
    {
        if (rewardNormalImage != null) rewardNormalImage.enabled = true;
        if (rewardClaimedImage != null) rewardClaimedImage.enabled = false;

        if (checkImage != null) checkImage.gameObject.SetActive(false);
        if (focusHighlight != null) focusHighlight.SetActive(true);
    }

    public void SetStateLocked()
    {
        if (rewardNormalImage != null) rewardNormalImage.enabled = true;
        if (rewardClaimedImage != null) rewardClaimedImage.enabled = false;

        if (checkImage != null) checkImage.gameObject.SetActive(false);
        if (focusHighlight != null) focusHighlight.SetActive(false);
    }
}
