using System;
using UnityEngine;

[RequireComponent(typeof(TowerRunStats))]
public class TowerLifeController : MonoBehaviour, IDamageable
{
    public TowerRunStats stats;
    public Animator animator;   // nếu muốn play anim chết

    private bool isDead = false;

    /// <summary>
    /// Event Tower chết – DeathController sẽ lắng nghe.
    /// </summary>
    public event Action OnDeath;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<TowerRunStats>();
    }

    public void TakeDamage(float damage)
    {
        if (stats == null) return;
        //if (isDead) return;
        if (damage <= 0f) return;

        stats.currentHP -= damage;
        Debug.Log($"Tower TakeDamage {damage}, HP còn {stats.currentHP}");

        if (stats.currentHP <= 0f)
        {
            stats.currentHP = 0f;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Anim chết nếu có
        if (animator != null)
            animator.SetTrigger("Die");

        Debug.Log("[TowerLifeController] Tower chết → bắn OnDeath event");

        // Bắn event cho DeathController
        OnDeath?.Invoke();

        // Nếu muốn, bắn luôn event OnDeath trong stats (UI khác có thể lắng nghe)
        //stats.OnDeath?.Invoke();

        // Disable các script tấn công nếu có
        // GetComponent<TowerAttack>()?.enabled = false;
    }

    /// <summary>
    /// Hồi sinh trụ: reset isDead + hồi full HP + bật lại logic.
    /// Gọi từ DeathController khi bấm nút Revive.
    /// </summary>
    public void Revive()
    {
        if (stats == null)
        {
            Debug.LogWarning("[TowerLifeController] Không có TowerRunStats để revive!");
            return;
        }

        isDead = false;

        // Hồi full bằng hàm chuẩn trong stats
        stats.ReviveToFull();

        // Reset animator nếu cần
        // if (animator != null) animator.Play("Idle");

        // Bật lại script tấn công/AI nếu đã tắt khi chết
        // GetComponent<TowerAttack>()?.enabled = true;

        Debug.Log("[TowerLifeController] Revive() → Tower sống lại, tiếp tục màn chơi");
    }
}
