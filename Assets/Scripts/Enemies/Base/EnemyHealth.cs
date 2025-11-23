using System;
using UnityEngine;
using Son.Economy;

[RequireComponent(typeof(EnemyGoldDrop))]
[RequireComponent(typeof(EnemyExpDropTest))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;

    private EnemyAI enemyAI;
    private LizardWarriorAI bossAI;
    private EnemyGoldDrop goldDrop;
    private EnemyExpDropTest expDrop;
    private float maxHealth;
    private bool isDead = false;

    public bool IsDead() => isDead;

    public Action onDie;
    public Action onHit;
    public Action<float, float> onHealthChanged;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        bossAI = GetComponent<LizardWarriorAI>();
        maxHealth = enemyAI != null ? enemyAI.stats.maxHealth : 100f;
        currentHealth = maxHealth;

        goldDrop = GetComponent<EnemyGoldDrop>();
        expDrop = GetComponent<EnemyExpDropTest>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(50f);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0 || isDead) return;

        float finalDamage = ApplyDamageReduction(damage);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        GetComponent<DamagePopupReceiver>()?.ShowDamage(finalDamage, transform.position);

        if (currentHealth > 0)
        {
            onHit?.Invoke();
            enemyAI.GetHit();
        }
        else
        {
            Die();
        }
    }

    private float ApplyDamageReduction(float damage)
    {
        if (bossAI != null)
            return damage * bossAI.GetDamageReductionMultiplier();
        return damage;
    }

    public float GetHealthPercent() => maxHealth > 0 ? currentHealth / maxHealth : 1f;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public void Heal(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        enemyAI.StopAI();
        onDie?.Invoke();

        HandleRewards();
    }

    private void HandleRewards()
    {
        expDrop.SetExpAmount(enemyAI.stats.expAmount);
        int goldGain = goldDrop?.DropGoldAndReturnAmount() ?? 0;
        expDrop?.OnEnemyKilled();

        if (goldGain > 0 && GoldPopupSpawner.Instance != null)
        {
            GoldPopupSpawner.Instance.SpawnGoldPopup(transform.position, goldGain);
            Debug.Log($"[EnemyHealth] Spawn gold popup: {goldGain}");
        }
    }

    public void OnDieAnimationEnd()
    {
        SpawnManager.Instance.OnEnemyKilled();
        PoolManager.Instance.Return(gameObject, enemyAI.stats.prefab);
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
    }
}
