using UnityEngine;

/// <summary>
/// Gắn script này lên Enemy để khi Enemy chết sẽ thưởng Gold cho người chơi.
/// Bạn chỉ cần gọi phương thức OnEnemyKilled() từ logic chết của Enemy.
/// </summary>
public class EnemyGoldDrop : MonoBehaviour
{
    [Header("Cấu hình Gold rơi khi quái chết")]
    [Tooltip("Gold rơi tối thiểu.")]
    public int minGold = 5;

    [Tooltip("Gold rơi tối đa.")]
    public int maxGold = 15;

    [Tooltip("Có log ra console khi quái rơi Gold không (debug).")]
    public bool logOnDrop = false;

    private bool _hasDropped = false;

    /// <summary>
    /// Gọi hàm này khi Enemy chính thức chết (ví dụ trong OnDeath).
    /// </summary>
    public void OnEnemyKilled()
    {
        if (_hasDropped)
        {
            // Tránh gọi 2 lần nếu death logic chạy trùng.
            return;
        }
        _hasDropped = true;

        if (WalletManager.Instance == null)
        {
            Debug.LogError("[EnemyGoldDrop] WalletManager.Instance == null. Không thể cộng Gold.");
            return;
        }

        if (maxGold < minGold)
        {
            maxGold = minGold;
        }

        int amount = Random.Range(minGold, maxGold + 1); // max +1 vì Random.Range int là [min, max)
        if (amount <= 0)
        {
            return;
        }

        WalletManager.Instance.AddCurrency(CurrencyType.Gold, amount, $"Enemy killed: {gameObject.name}");

        if (logOnDrop)
        {
            Debug.Log($"[EnemyGoldDrop] {gameObject.name} rơi {amount} Gold.");
        }

        // Sau này nếu muốn spawn coin vật lý/visual thì bạn có thể thêm ở đây.
    }
}
