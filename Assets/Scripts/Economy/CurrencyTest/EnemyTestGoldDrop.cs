using UnityEngine;

/// <summary>
/// Enemy test dùng để kiểm tra hệ thống:
/// - Máu / Die()
/// - Cộng Gold vào WalletManager
/// - Hiển thị popup Gold thông qua GoldPopupSpawner
///
/// Cách dùng:
/// - Gắn script này lên 1 GameObject (enemy test) trong scene.
/// - Nhấn Play, rồi bấm phím K để giết enemy và xem gold + popup.
/// </summary>
public class EnemyTestGoldDrop : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Máu tối đa của enemy test.")]
    public float maxHealth = 100f;

    [Tooltip("Máu hiện tại (readonly trong runtime).")]
    [SerializeField] private float currentHealth;

    [Header("Gold Reward")]
    [Tooltip("Gold rơi tối thiểu khi enemy chết.")]
    public int minGoldReward = 20;

    [Tooltip("Gold rơi tối đa khi enemy chết.")]
    public int maxGoldReward = 50;

    [Header("Test Input")]
    [Tooltip("Phím dùng để giết enemy test (dành cho debug).")]
    public KeyCode killKey = KeyCode.K;

    private bool _isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Nhấn K để kill ngay lập tức (test)
        if (Input.GetKeyDown(killKey))
        {
            Debug.Log("[EnemyTestGoldDrop] Kill key pressed → Die()");
            Die();
        }
    }

    /// <summary>
    /// Gọi hàm này khi enemy nhận sát thương (nếu bạn muốn test bằng damage thật).
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        if (damage <= 0f) return;

        currentHealth -= damage;
        Debug.Log($"[EnemyTestGoldDrop] TakeDamage: {damage} | HP = {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Xử lý khi enemy chết:
    /// - Tính Gold reward
    /// - Cộng vào WalletManager
    /// - Gọi popup thông qua GoldPopupSpawner
    /// </summary>
    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // Tính gold thưởng
        int min = Mathf.Max(0, minGoldReward);
        int max = Mathf.Max(min, maxGoldReward);
        int goldGain = Random.Range(min, max + 1);

        // 1) Cộng Gold vào ví
        if (goldGain > 0 && WalletManager.Instance != null)
        {
            WalletManager.Instance.AddCurrency(
                CurrencyType.Gold,
                goldGain,
                "EnemyTestGoldDrop killed"
            );
        }
        else
        {
            if (WalletManager.Instance == null)
                Debug.LogWarning("[EnemyTestGoldDrop] WalletManager.Instance == null, không thể cộng Gold.");
        }

        // 2) Hiển thị popup Gold (coin + text)
        if (goldGain > 0 && GoldPopupSpawner.Instance != null)
        {
            GoldPopupSpawner.Instance.SpawnGoldPopup(transform.position, goldGain);
            Debug.Log("Nhay popup");
        }
        else
        {
            if (GoldPopupSpawner.Instance == null)
                Debug.LogWarning("[EnemyTestGoldDrop] GoldPopupSpawner.Instance == null, không thể spawn popup.");
        }

        // TODO: FX chết / animation nếu cần

        Debug.Log($"[EnemyTestGoldDrop] Enemy chết, thưởng {goldGain} Gold.");
        Destroy(gameObject);
    }
}
