using UnityEngine;

public class PlayerDeathTester : MonoBehaviour
{
    [Header("HP Test")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Phím test")]
    [Tooltip("Ấn phím này để trừ máu (ví dụ -10 mỗi lần).")]
    public KeyCode damageKey = KeyCode.O;

    [Tooltip("Lượng máu trừ mỗi lần nhấn damageKey.")]
    public int damageAmount = 10;

    [Tooltip("Ấn phím này để chết ngay lập tức (set HP = 0).")]
    public KeyCode instantDeathKey = KeyCode.P;

    private bool isDead = false;

    private void Start()
    {
        // Khởi tạo máu về max khi bắt đầu
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Test: nhấn K để trừ máu
        if (Input.GetKeyDown(damageKey))
        {
            TakeDamage(damageAmount);
        }

        // Test: nhấn P để chết ngay
        if (Input.GetKeyDown(instantDeathKey))
        {
            TakeDamage(currentHealth); // trừ hết máu
        }
    }

    /// <summary>
    /// Gọi để trừ máu (chỉ dùng cho test).
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"[PlayerDeathTester] TakeDamage {amount} -> HP = {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// Khi máu = 0 -> die, hiện panel death.
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerDeathTester] PLAYER DEAD (test).");

        if (Son.Economy.DeathPanelController.Instance != null)
        {
            Son.Economy.DeathPanelController.Instance.Show(); // panel death + pause game
        }
        else
        {
            Debug.LogError("[PlayerDeathTester] DeathPanelController.Instance == null");
        }
    }

    /// <summary>
    /// Hàm này dùng cho nút Revive (Continue) trong panel death.
    /// </summary>
    public void RevivePlayer()
    {
        isDead = false;
        currentHealth = maxHealth;

        Debug.Log($"[PlayerDeathTester] REVIVE -> HP = {currentHealth}");

        // Không cần hide panel ở đây vì DeathPanelController
        // đã Hide() trước khi gọi onContinue.
        // Nếu muốn chắc chắn, có thể:
        // DeathPanelController.Instance?.Hide();
    }
}
