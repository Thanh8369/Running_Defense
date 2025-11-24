using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    //[SerializeField] private int _maxHealth = 100;
    public PlayerHpData playerHpData;
    public float MaxHealth => playerHpData._maxHealth;
    public float CurrentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnRevive;

    private void Awake()
    {
        CurrentHealth = playerHpData._maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);
        Debug.Log($"[Health] Awake, HP = {CurrentHealth}/{playerHpData._maxHealth}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && gameObject.CompareTag("Player"))
        {
            TakeDamage(MaxHealth);
        }

        if (Input.GetKeyDown(KeyCode.T) && gameObject.CompareTag("Tower"))
        {
            TakeDamage(MaxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0 || CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, playerHpData._maxHealth);

        Debug.Log($"[Health] TakeDamage {damage}, HP = {CurrentHealth}/{playerHpData._maxHealth}");

        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("[Health] Dead");
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, playerHpData._maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);
    }

    /// <summary>
    /// Dùng cho revive: set thẳng máu về full, kể cả khi đang = 0.
    /// Chỉ DeathController của tower gọi hàm này.
    /// </summary>
    public void ReviveToFull()
    {
        CurrentHealth = playerHpData._maxHealth;
        Debug.Log($"[Health] ReviveToFull, HP = {CurrentHealth}/{playerHpData._maxHealth}");
        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);
        OnRevive?.Invoke();
    }
}
