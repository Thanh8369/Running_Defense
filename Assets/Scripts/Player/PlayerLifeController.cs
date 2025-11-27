using Son.Economy;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Quản lý trạng thái sống/chết + revive của player.
/// - HP lấy từ PlayerRunStats.currentHP / maxHP.
/// - Khi chết: khóa di chuyển, tấn công, nhận exp, lướt + play anim Die.
/// - Khi revive: chờ reviveDelay (nếu > 0), hồi máu dần trong reviveDuration,
///   reset anim về Idle + mở lại điều khiển.
/// </summary>
public class PlayerLifeController : MonoBehaviour, IDamageable   // 👈 THÊM IDamageable
{
    public static PlayerLifeController Instance { get; private set; }

    [Header("Tham chiếu HP (runtime stats)")]
    public PlayerRunStats runStats;             // chứa currentHP, maxHP

    [Header("Exp / hệ thống khác")]
    public PlayerExperienceManager expManager;  // để chặn nhận exp khi chết

    [Header("Các hệ thống cần khóa khi chết")]
    public TapToMoveController moveController;  // di chuyển + roll
    public DaggerATK daggerAtk;                // tấn công cận chiến
    public testAutoarrow autoArrow;            // bắn tự động tầm xa

    [Header("Animation & Revive")]
    public Animator animator;
    [Tooltip("Thời gian chờ trước khi bắt đầu hồi máu (giống reviveDelay ở PlayerDeathController cũ).")]
    public float reviveDelay = 0f;

    [Tooltip("Thời gian nội tại để hồi đầy máu (lerp).")]
    public float reviveDuration = 2f;

    // Trạng thái
    private bool isDead = false;
    private bool isReviving = false;

    // Hash animation, lấy lại từ PlayerDeathController cũ
    private readonly int AnimDie = Animator.StringToHash("Die");
    private readonly int AnimIdle = Animator.StringToHash("Idle");

    /// <summary>Player đã chết chưa?</summary>
    public bool IsDead => isDead;

    /// <summary>Player đang trong quá trình revive không?</summary>
    public bool IsReviving => isReviving;

    /// <summary>Player có được phép hành động không?</summary>
    public bool CanAct => !isDead && !isReviving;

    private bool isInvulnerable = false;

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;

    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Auto tìm tham chiếu nếu quên gán
        if (runStats == null) runStats = GetComponent<PlayerRunStats>();
        if (moveController == null) moveController = GetComponent<TapToMoveController>();
        if (daggerAtk == null) daggerAtk = GetComponent<DaggerATK>();
        if (autoArrow == null) autoArrow = GetComponent<testAutoarrow>();
        if (expManager == null) expManager = FindAnyObjectByType<PlayerExperienceManager>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    // ==========================
    // IDamageable
    // ==========================
    /// <summary>
    /// Implement IDamageable. Các script khác chỉ cần gọi TakeDamage() là được.
    /// </summary>
    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);
    }

    // ==========================
    // GỌI HÀM NÀY KHI PLAYER ĂN DAMAGE (internal)
    // ==========================
    public void ApplyDamage(float amount)
    {
        Debug.Log($"Player TakeDamage {amount}, HP trước = {runStats.currentHP}");

        if (runStats == null) return;

        // chết rồi, đang revive, hoặc đang bất tử (roll) thì không ăn damage
        if (isDead || isReviving || isInvulnerable) return;
        if (amount <= 0f) return;

        runStats.currentHP -= amount;
        if (runStats.currentHP <= 0f)
        {
            runStats.currentHP = 0f;
            Die();
        }
    }

    // ==========================
    // HANDLE DIE
    // ==========================
    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isReviving = false;

        Debug.Log("[PlayerLife] Player died → lock control + play Die anim");

        // 1) Tắt input / movement / tấn công / exp
        DisableControl();

        // 2) Play die animation
        if (animator != null)
        {
            animator.SetTrigger(AnimDie);
        }

        // 3) Auto revive sau 1 thời gian
        StartRevive();

        OnDeath?.Invoke();
    }

    // ==========================
    // GỌI TỪ UI HOẶC TỰ GỌI TRONG Die()
    // ==========================
    public void StartRevive()
    {
        if (!isDead) return;

        StopAllCoroutines();
        StartCoroutine(ReviveRoutine());
    }

    // ==========================
    // REVIVE ROUTINE
    // ==========================
    private IEnumerator ReviveRoutine()
    {
        isReviving = true;
        DisableControl(); // đang revive vẫn khóa hết

        // 1) Chờ reviveDelay nếu có
        if (reviveDelay > 0f)
        {
            yield return new WaitForSeconds(reviveDelay);
        }

        // 2) Hồi máu dần từ currentHP → maxHP trong reviveDuration
        float startHP = runStats.currentHP; // thường = 0
        float endHP = runStats.maxHP;
        float t = 0f;

        while (t < reviveDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / reviveDuration);
            runStats.currentHP = Mathf.Lerp(startHP, endHP, k);

            // TODO: update UI HP nếu cần

            yield return null;
        }

        runStats.currentHP = endHP;

        // 3) Reset trạng thái Animator
        if (animator != null)
        {
            animator.ResetTrigger(AnimDie);
            animator.Play(AnimIdle, 0, 0f);
        }

        // 4) Mở lại điều khiển
        isDead = false;
        isReviving = false;
        EnableControl();
        OnRevive?.Invoke();

        Debug.Log("[PlayerLife] Revive done → unlock control");
    }

    // ==========================
    // BẬT / TẮT CONTROL
    // ==========================
    private void SetPlayerActive(bool canAct)
    {
        // Di chuyển + lướt
        if (moveController != null)
        {
            moveController.enabled = canAct;

            if (!canAct)
            {
                if (moveController.agent != null)
                {
                    moveController.agent.isStopped = true;
                    moveController.agent.velocity = Vector3.zero;
                }
                if (moveController.animator != null)
                {
                    moveController.animator.SetFloat("Speed", 0f);
                }
            }
        }

        // Tấn công cận chiến
        if (daggerAtk != null)
            daggerAtk.enabled = canAct;

        // Tấn công tầm xa
        if (autoArrow != null)
            autoArrow.enabled = canAct;

        // Nhận EXP
        if (expManager != null)
            expManager.canGainExp = canAct;
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    private void DisableControl() => SetPlayerActive(false);
    private void EnableControl() => SetPlayerActive(true);
}
