using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hiển thị số lượng 1 loại tiền cụ thể (Gold hoặc Gem) trên UI.
/// Gắn script này vào GameObject group, ví dụ: Gold_Group, Gem_Group.
/// WalletManager.Instance.AddCurrency(CurrencyType.Gold, dropAmount, "Enemy kill");     KHI QUÁI CHẾT
/// WalletManager.Instance.AddCurrency(CurrencyType.Gem, gemReward, "Victory reward");   KHI THẮNG TRẬN
/// </summary>
public class CurrencyUI : MonoBehaviour
{
    [Header("Loại tiền mà UI này hiển thị")]
    public CurrencyType currencyType = CurrencyType.Gold;

    [Header("Tham chiếu các component UI")]
    [Tooltip("Icon hiển thị tiền tệ (không bắt buộc).")]
    public Image iconImage;

    [Tooltip("Text hiển thị số lượng.")]
    public TextMeshProUGUI amountText;

    [Header("Format hiển thị")]
    [Tooltip("Có dùng format rút gọn không? VD: 1.2K, 3.5M")]
    public bool useShortFormat = true;

    //Sử dụng khi có scene bootrap để khởi tạo wall manager
    private void OnEnable()
    {
        // Đăng ký event với WalletManager khi object được bật.
        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

            // Cập nhật giá trị ban đầu cho đúng.
            int currentBalance = WalletManager.Instance.GetBalance(currencyType);
            UpdateAmountText(currentBalance);
        }
        else
        {
            Debug.LogWarning($"[CurrencyUI] WalletManager.Instance chưa tồn tại khi OnEnable (type={currencyType}).");
        }
    }

    //Sử dụng để Test Curr ở Scene Test
    //private void OnEnable()
    //{
    //    StartCoroutine(WaitForWalletAndInit());
    //}

    //private IEnumerator WaitForWalletAndInit()
    //{
    //    // Đợi WalletManager.Instance sẵn sàng
    //    while (WalletManager.Instance == null)
    //        yield return null;

    //    WalletManager.Instance.OnCurrencyChanged += HandleCurrencyChanged;

    //    // Update giá trị ban đầu
    //    int currentBalance = WalletManager.Instance.GetBalance(currencyType);
    //    UpdateAmountText(currentBalance);
    //}

    private void OnDisable()
    {
        // Hủy đăng ký event khi object bị tắt.
        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }

    /// <summary>
    /// Hàm được gọi khi WalletManager báo số dư thay đổi.
    /// </summary>
    private void HandleCurrencyChanged(CurrencyType changedType, int newAmount)
    {
        // Chỉ quan tâm tới loại tiền mà UI này đang hiển thị.
        if (changedType != currencyType) return;

        UpdateAmountText(newAmount);
    }

    /// <summary>
    /// Cập nhật text hiển thị.
    /// </summary>
    private void UpdateAmountText(int amount)
    {
        if (amountText == null)
        {
            Debug.LogWarning($"[CurrencyUI] amountText chưa được gán (type={currencyType}).");
            return;
        }

        if (useShortFormat)
        {
            amountText.text = FormatShortNumber(amount);
        }
        else
        {
            amountText.text = amount.ToString();
        }
    }

    /// <summary>
    /// Format số lớn cho gọn (VD: 1.2K, 3.4M...).
    /// </summary>
    private string FormatShortNumber(int number)
    {
        if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString("0.#") + "B";
        if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "K";

        return number.ToString();
    }
}
