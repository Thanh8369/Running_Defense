using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public Health health;
    public Animator animator;
    public float reviveDelay = 15f;

    private bool isDead = false;

    private readonly int AnimDie = Animator.StringToHash("Die");
    private readonly int AnimIdle = Animator.StringToHash("Idle");

    private void Start()
    {
        health.OnDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[PlayerDeath] Player died → Play animation + disable movement");

        // 1) Tắt input / movement
        DisableControl();

        // 2) Play die animation
        animator.SetTrigger(AnimDie);

        // 3) Bắt đầu đếm giờ revive
        StartCoroutine(ReviveAfterDelay());
    }

    private System.Collections.IEnumerator ReviveAfterDelay()
    {
        yield return new WaitForSeconds(reviveDelay);

        // 4) Hồi máu
        health.ReviveToFull();

        // 5) Reset trạng thái Animator
        animator.ResetTrigger(AnimDie);
        animator.Play(AnimIdle, 0, 0f);

        // 6) Mở lại điều khiển
        EnableControl();

        isDead = false;

        Debug.Log("[PlayerDeath] Revived!");
    }

    private void DisableControl()
    {
        // TÙY GAME CỦA BẠN, ví dụ:
        var movement = GetComponent<TapToMoveController>();
        if (movement) movement.enabled = false;

        var combat = GetComponent<TapToMoveController>();
        if (combat) combat.enabled = false;
    }

    private void EnableControl()
    {
        var movement = GetComponent<TapToMoveController>();
        if (movement) movement.enabled = true;

        var combat = GetComponent<TapToMoveController>();
        if (combat) combat.enabled = true;
    }
}
