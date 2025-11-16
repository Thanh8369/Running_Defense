using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 100;
    public float MaxHealth => _maxHealth;
    public float CurrentHealth { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = _maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
        Debug.Log($"[Health] Awake, HP = {CurrentHealth}/{_maxHealth}");
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0 || CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);

        Debug.Log($"[Health] TakeDamage {damage}, HP = {CurrentHealth}/{_maxHealth}");

        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

        if (CurrentHealth <= 0)
        {
            Debug.Log("[Health] Dead");
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, _maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
    }
}
