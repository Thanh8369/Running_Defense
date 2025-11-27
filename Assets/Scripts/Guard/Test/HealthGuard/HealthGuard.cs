using System;
using UnityEngine;

public class HealthGuard : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    public event Action OnDeath;

    private GuardAnimation guardAnim;
    public bool isDead { get; private set; } = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        guardAnim = GetComponent<GuardAnimation>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0 || CurrentHealth <= 0)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Gọi animation Die
        if (guardAnim != null)
        {
            guardAnim.PlayDie();
        }

        OnDeath?.Invoke();

        // Xóa object sau 1 giây
        Destroy(gameObject, 4f);
    }
}