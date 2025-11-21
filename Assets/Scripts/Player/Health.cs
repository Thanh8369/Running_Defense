using System;
using UnityEngine;
using Son.Economy;   // <- thêm dòng này để dùng DeathPanelController

public class Health : MonoBehaviour, IDamageable
{
    public PlayerHpData playerHpData;
    public float MaxHealth => playerHpData._maxHealth;
    public float CurrentHealth { get; private set; }

    /// <summary>
    /// float current, float max
    /// </summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>
    /// Gọi khi chết
    /// </summary>
    public event Action OnDeath;

    private void Update()
    {
        // Test: nhấn P để chết ngay
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(CurrentHealth); // trừ hết máu
        }
    }

    private void Awake()
    {
        CurrentHealth = playerHpData._maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);
        Debug.Log($"[Health] Awake, HP = {CurrentHealth}/{playerHpData._maxHealth}");
    }

    private void OnEnable()
    {
        // Khi Health này chết → gọi HandleDeath()
        OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        OnDeath -= HandleDeath;
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
            OnDeath?.Invoke();   // bắn event chết
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, playerHpData._maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, playerHpData._maxHealth);
    }

    /// <summary>
    /// Hàm xử lý khi chết: hiện Death Panel
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log("[Health] HandleDeath -> Show DeathPanel");

        if (DeathPanelController.Instance != null)
        {
            DeathPanelController.Instance.Show();
        }
        else
        {
            Debug.LogWarning("[Health] Không tìm thấy DeathPanelController.Instance trong scene!");
        }
    }

    public void Revive()
    {
        var health = GetComponent<Health>();
        health.Heal(health.MaxHealth);
        // bật lại movement, AI... nếu có
    }
}
