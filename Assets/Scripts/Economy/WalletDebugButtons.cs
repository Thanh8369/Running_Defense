using UnityEngine;

/// <summary>
/// Script dùng để gán vào UI Button trong Editor,
/// giúp test nhanh Add/Spend Gold & Gem.
/// </summary>
public class WalletDebugButtons : MonoBehaviour
{
    [Header("Tham chiếu WalletManager (nếu không dùng singleton)")]
    public WalletManager walletManagerOverride;

    private WalletManager Wallet =>
        walletManagerOverride != null ? walletManagerOverride : WalletManager.Instance;

    private bool IsWalletReady()
    {
        if (Wallet == null)
        {
            Debug.LogError("[WalletDebugButtons] WalletManager chưa sẵn sàng!");
            return false;
        }
        return true;
    }

    // ===== GOLD =====

    public void AddGold100()
    {
        if (!IsWalletReady()) return;
        Wallet.AddCurrency(CurrencyType.Gold, 100, "Debug Button: +100 Gold");
    }

    public void SpendGold100()
    {
        if (!IsWalletReady()) return;
        bool success = Wallet.SpendCurrency(CurrencyType.Gold, 100, "Debug Button: -100 Gold");
        if (!success)
        {
            Debug.Log("[WalletDebugButtons] Không đủ Gold để trừ 100.");
        }
    }

    // ===== GEM =====

    public void AddGem10()
    {
        if (!IsWalletReady()) return;
        Wallet.AddCurrency(CurrencyType.Gem, 10, "Debug Button: +10 Gem");
    }

    public void SpendGem10()
    {
        if (!IsWalletReady()) return;
        bool success = Wallet.SpendCurrency(CurrencyType.Gem, 10, "Debug Button: -10 Gem");
        if (!success)
        {
            Debug.Log("[WalletDebugButtons] Không đủ Gem để trừ 10.");
        }
    }

    // Optional: in toàn bộ balance ra log
    public void PrintBalances()
    {
        if (!IsWalletReady()) return;

        int gold = Wallet.GetBalance(CurrencyType.Gold);
        int gem = Wallet.GetBalance(CurrencyType.Gem);

        Debug.Log($"[WalletDebugButtons] BALANCES → Gold={gold}, Gem={gem}");
    }
}
