using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Gắn script này lên Enemy để khi Enemy chết sẽ thưởng Gold cho người chơi.
    /// Bạn chỉ cần gọi phương thức OnEnemyKilled() từ logic chết của Enemy.
    /// </summary>
    public class EnemyGoldDrop : MonoBehaviour
    {
        [Header("Tham chiếu Stats (ScriptableObject)")]
        [Tooltip("EnemyStats dùng để đọc minGold / maxGold.")]
        public EnemyAI enemyStats { get; private set; }


        [Header("Debug")]
        [Tooltip("Có log ra console khi quái rơi Gold không (debug).")]
        public bool logOnDrop = false;

        private bool _hasDropped = false;

        /// <summary>
        /// Gold đã rơi lần gần nhất (0 nếu chưa rơi / không rơi).
        /// </summary>
        public int LastDropAmount { get; private set; }

        private void Start()
        {
            enemyStats = GetComponent<EnemyAI>();
        }

        /// <summary>
        /// Optional: nếu spawn code muốn gán stats sau khi Instantiate.
        /// </summary>
        public void SetStats(EnemyStats stats)
        {
            enemyStats.stats = stats;
        }

        /// <summary>
        /// PUBLIC cũ: vẫn giữ để code khác dùng được.
        /// Không trả về gì.
        /// </summary>
        public void OnEnemyKilled()
        {
            DropGoldAndReturnAmount();
        }

        /// <summary>
        /// Hàm mới: rơi Gold và trả về số Gold đã cộng vào Wallet.
        /// EnemyHealth sẽ dùng hàm này để hiện popup.
        /// </summary>
        public int DropGoldAndReturnAmount()
        {
            if (_hasDropped)
            {
                // Tránh gọi 2 lần nếu death logic chạy trùng.
                return 0;
            }
            _hasDropped = true;
            LastDropAmount = 0;

            if (WalletManager.Instance == null)
            {
                Debug.LogError("[EnemyGoldDrop] WalletManager.Instance == null. Không thể cộng Gold.");
                return 0;
            }

            if (enemyStats == null)
            {
                Debug.LogError("[EnemyGoldDrop] enemyStats == null. Gán EnemyStats vào EnemyGoldDrop trong Inspector hoặc qua code.");
                return 0;
            }

            // Lấy min / max từ EnemyStats
            int minGold = enemyStats.stats.minGold;
            int maxGold = enemyStats.stats.maxGold;

            if (maxGold < minGold)
            {
                maxGold = minGold;
            }

            int amount = Random.Range(minGold, maxGold + 1); // Random.Range int là [min, maxExclusive)
            if (amount <= 0)
            {
                return 0;
            }

            LastDropAmount = amount;

            WalletManager.Instance.AddCurrency(
                CurrencyType.Gold,
                amount,
                $"Enemy killed: {gameObject.name}"
            );

            if (logOnDrop)
            {
                Debug.Log($"[EnemyGoldDrop] {gameObject.name} rơi {amount} Gold (min:{minGold}, max:{maxGold}).");
            }

            return amount;
        }
    }
}
