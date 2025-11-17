using System;
using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Thực hiện mua/upgrade: kiểm tra đủ tiền, trừ tiền, tăng level, phát event.
    /// </summary>
    public class PurchaseService : MonoBehaviour
    {
        public static PurchaseService Instance { get; private set; }

        [Header("Deps")]
        public ShopState shopState;      // auto find nếu để trống
        public WalletManager wallet;     // dùng WalletManager từ Day 3

        // Events (không dùng [Header] cho event)
        public event Action<ShopItemConfig, int> OnPurchased;           // (item, newLevel)
        public event Action<ShopItemConfig, string> OnPurchaseFailed;   // (item, reason)

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        private void Start()
        {
            if (shopState == null) shopState = ShopState.Instance;
            if (wallet == null) wallet = WalletManager.Instance;

            if (shopState == null) Debug.LogError("[PurchaseService] Missing ShopState in scene.");
            if (wallet == null) Debug.LogError("[PurchaseService] Missing WalletManager in scene.");
        }

        public bool TryPurchase(ShopItemConfig item, out string error)
        {
            error = string.Empty;
            if (item == null) { error = "Item null"; return false; }
            if (shopState == null || wallet == null) { error = "Thiếu dịch vụ"; return false; }

            int curLv = shopState.GetLevel(item.id);
            if (curLv >= item.maxLevel)
            {
                error = "Đã đạt cấp tối đa.";
                OnPurchaseFailed?.Invoke(item, error);
                return false;
            }

            int cost = item.GetCost(curLv);
            if (!wallet.HasEnough(item.currency, cost))
            {
                error = $"Không đủ {item.currency} (cần {cost}).";
                OnPurchaseFailed?.Invoke(item, error);
                return false;
            }

            // Sẽ dùng extension Subtract() nếu bạn áp dụng Cách A ở trên
            if (!wallet.SpendCurrency(item.currency, cost, reason: $"Buy:{item.id}@{curLv}->{curLv + 1}"))
            {
                error = "Trừ tiền thất bại (đồng bộ ví?).";
                OnPurchaseFailed?.Invoke(item, error);
                return false;
            }

            int newLv = shopState.IncrementLevel(item.id, item.maxLevel);
            Debug.Log($"[Purchase] {item.id} level {curLv} -> {newLv}; -{cost} {item.currency}");
            OnPurchased?.Invoke(item, newLv);
            return true;
        }
    }
}
