using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý số dư tiền tệ của người chơi (Gold, Gem).
/// - Gold: soft currency, dùng trong run, không cần lưu vĩnh viễn.
/// - Gem : meta currency, cần lưu qua các lần mở game.
/// </summary>
public class WalletManager : MonoBehaviour
{
    // Singleton đơn giản để dùng ở mọi scene.
    public static WalletManager Instance { get; private set; }

    [Header("Database tiền tệ")]
    [Tooltip("Kéo CurrencyDatabase_Main vào đây.")]
    [SerializeField] private CurrencyDatabase currencyDatabase;

    /// <summary>
    /// Số dư theo từng loại tiền.
    /// </summary>
    private Dictionary<CurrencyType, int> _balances = new Dictionary<CurrencyType, int>();

    /// <summary>
    /// Event gọi khi số dư 1 loại tiền thay đổi.
    /// Dùng để UI đăng ký lắng nghe.
    /// </summary>
    public event Action<CurrencyType, int> OnCurrencyChanged;

    // Khóa dùng cho PlayerPrefs.
    private const string WALLET_SAVE_KEY = "RD_WalletSave_v1";

    private void Awake()
    {
        // Đảm bảo chỉ có 1 instance.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Khởi tạo số dư (0 hết) rồi Load Gem từ save.
        InitBalances();
        LoadWallet();
    }

    /// <summary>
    /// Khởi tạo số dư ban đầu cho tất cả loại tiền (0).
    /// </summary>
    private void InitBalances()
    {
        _balances = new Dictionary<CurrencyType, int>();

        // Đảm bảo có currencyDatabase để loop.
        if (currencyDatabase == null)
        {
            Debug.LogError("[WalletManager] Chưa gán CurrencyDatabase!");
            return;
        }

        foreach (var currencyData in currencyDatabase.currencies)
        {
            if (currencyData == null) continue;

            var type = currencyData.currencyType;
            if (!_balances.ContainsKey(type))
            {
                _balances[type] = 0;
            }
        }

        Debug.Log("[WalletManager] Init balances xong (tất cả = 0).");
    }

    #region PUBLIC API (Get / Has / Add / Spend)

    /// <summary>
    /// Lấy số dư hiện tại của 1 loại tiền.
    /// </summary>
    public int GetBalance(CurrencyType type)
    {
        if (_balances.TryGetValue(type, out var value))
        {
            return value;
        }

        // Nếu chưa có key, mặc định là 0.
        return 0;
    }

    /// <summary>
    /// Kiểm tra xem có đủ tiền không.
    /// </summary>
    public bool HasEnough(CurrencyType type, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("[WalletManager] HasEnough nhận amount âm.");
            return false;
        }

        return GetBalance(type) >= amount;
    }

    /// <summary>
    /// Cộng tiền. amount có thể âm nhưng nên dùng SpendCurrency thay vì truyền âm.
    /// Nếu thay đổi Gem -> auto Save.
    /// </summary>
    public void AddCurrency(CurrencyType type, int amount, string reason = "")
    {
        if (!_balances.ContainsKey(type))
        {
            _balances[type] = 0;
        }

        if (amount == 0)
        {
            return;
        }

        int oldBalance = _balances[type];
        long newBalanceLong = (long)oldBalance + amount;

        // Clamp không cho xuống âm.
        if (newBalanceLong < 0)
        {
            newBalanceLong = 0;
            Debug.LogWarning($"[WalletManager] AddCurrency khiến balance âm, đã clamp về 0. Type={type}");
        }

        int newBalance = (int)newBalanceLong;
        _balances[type] = newBalance;

        // Log debug
        string reasonText = string.IsNullOrEmpty(reason) ? "" : $" | Reason: {reason}";
        Debug.Log($"[WalletManager] {(amount >= 0 ? "ADD" : "SUB")} {Mathf.Abs(amount)} {type} | " +
                  $"Old={oldBalance} → New={newBalance}{reasonText}");

        // Gọi event cho UI, v.v.
        OnCurrencyChanged?.Invoke(type, newBalance);

        // Nếu là Gem (meta) thì lưu luôn.
        if (type == CurrencyType.Gem)
        {
            SaveWallet();
        }
    }

    /// <summary>
    /// Trừ tiền. Trả về true nếu trừ thành công, false nếu không đủ.
    /// </summary>
    public bool SpendCurrency(CurrencyType type, int amount, string reason = "")
    {
        if (amount < 0)
        {
            Debug.LogWarning("[WalletManager] SpendCurrency nhận amount âm. Hãy truyền số dương.");
            return false;
        }

        if (!HasEnough(type, amount))
        {
            Debug.Log($"[WalletManager] KHÔNG ĐỦ {type} để trừ {amount}. " +
                      $"Current={GetBalance(type)}{(string.IsNullOrEmpty(reason) ? "" : $" | Reason: {reason}")}");
            return false;
        }

        AddCurrency(type, -amount, reason);
        return true;
    }

    /// <summary>
    /// Reset tất cả tiền về 0 (hữu ích khi debug).
    /// Lưu ý: cũng sẽ set Gem=0 và ghi đè save.
    /// </summary>
    [ContextMenu("Reset All Balances To Zero")]
    public void ResetAllBalances()
    {
        var keys = new List<CurrencyType>(_balances.Keys);
        foreach (var key in keys)
        {
            _balances[key] = 0;
            OnCurrencyChanged?.Invoke(key, 0);
        }

        Debug.Log("[WalletManager] Đã reset tất cả balances về 0.");
        SaveWallet();
    }

    #endregion

    #region SAVE / LOAD

    /// <summary>
    /// Lưu dữ liệu ví (chỉ Gem) vào PlayerPrefs.
    /// </summary>
    public void SaveWallet()
    {
        var data = new WalletSaveData
        {
            gem = GetBalance(CurrencyType.Gem)
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(WALLET_SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[WalletManager] Đã SaveWallet → {json}");
    }

    /// <summary>
    /// Load dữ liệu ví (Gem) từ PlayerPrefs.
    /// </summary>
    private void LoadWallet()
    {
        if (!PlayerPrefs.HasKey(WALLET_SAVE_KEY))
        {
            Debug.Log("[WalletManager] Không tìm thấy save wallet, dùng mặc định (Gem=0).");
            return;
        }

        string json = PlayerPrefs.GetString(WALLET_SAVE_KEY, "");
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("[WalletManager] WalletSave json rỗng, bỏ qua.");
            return;
        }

        try
        {
            var data = JsonUtility.FromJson<WalletSaveData>(json);
            if (data == null)
            {
                Debug.LogWarning("[WalletManager] Deserialization WalletSaveData thất bại, bỏ qua.");
                return;
            }

            // Gán Gem từ save, clamp >=0
            int gemValue = Mathf.Max(0, data.gem);
            _balances[CurrencyType.Gem] = gemValue;

            // Thông báo cho UI.
            OnCurrencyChanged?.Invoke(CurrencyType.Gem, gemValue);

            Debug.Log($"[WalletManager] LoadWallet thành công: Gem={gemValue}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[WalletManager] Lỗi LoadWallet: {e.Message}");
        }
    }

    /// <summary>
    /// Khi app tạm dừng (Android) -> lưu lại.
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveWallet();
        }
    }

    /// <summary>
    /// Khi app thoát -> lưu lại lần nữa cho chắc.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveWallet();
    }

    #endregion

    #region RUN LOGIC HỖ TRỢ (CHO VỀ SAU)

    /// <summary>
    /// Gọi hàm này khi bắt đầu 1 run mới:
    /// - Reset Gold về 0.
    /// - Gem vẫn giữ nguyên (meta progression).
    /// </summary>
    public void ResetGoldForNewRun()
    {
        if (_balances.ContainsKey(CurrencyType.Gold))
        {
            _balances[CurrencyType.Gold] = 0;
            OnCurrencyChanged?.Invoke(CurrencyType.Gold, 0);
            Debug.Log("[WalletManager] ResetGoldForNewRun → Gold = 0.");
        }
    }

    #endregion
}
