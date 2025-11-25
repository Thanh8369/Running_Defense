using System;
using UnityEngine;
using Son.Economy;

[RequireComponent(typeof(EnemyGoldDrop))]
[RequireComponent(typeof(EnemyExpDropTest))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;

    public bool isBoss = false;

    private EnemyAI enemyAI;
    private LizardWarriorAI bossAI;
    private EnemyGoldDrop goldDrop;
    private EnemyExpDropTest expDrop;
    private DamagePopupReceiver damagePopupReceiver;
    private StageClearRewardUI stageClearRewardUI;

    private float maxHealth;
    private bool isDead = false;

    public bool IsDead() => isDead;

    public Action onDie;
    public Action onHit;
    public Action<float, float> onHealthChanged;

    private void OnEnable()
    {
        enemyAI = GetComponent<EnemyAI>();
        bossAI = GetComponent<LizardWarriorAI>();
        goldDrop = GetComponent<EnemyGoldDrop>();
        expDrop = GetComponent<EnemyExpDropTest>();
        damagePopupReceiver = GetComponent<DamagePopupReceiver>();

        stageClearRewardUI = FindAnyObjectByType<StageClearRewardUI>(FindObjectsInactive.Include);

        maxHealth = enemyAI.stats.maxHealth;
        currentHealth = maxHealth;
        isDead = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(maxHealth);
        }
    }

    public void Heal(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0 || isDead) return;

        float finalDamage = ApplyDamageReduction(damage);
        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onHealthChanged?.Invoke(currentHealth, maxHealth);
        damagePopupReceiver?.ShowDamage(finalDamage, transform.position);

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
            GoldPopupSpawner.Instance.SpawnGoldPopup(transform.position, goldGain);
    }

    public void OnDieAnimationEnd()
    {
        if (isBoss)
        {
            EnemySpawner.Instance.AddStar();
            stageClearRewardUI.ShowReward(EnemySpawner.Instance.TotalStars, 0);
            Debug.LogWarning($"Boss defeated. Total Stars: {EnemySpawner.Instance.TotalStars}");
        }

        PoolManager.Instance.Return(gameObject, enemyAI.stats.prefab);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
}
