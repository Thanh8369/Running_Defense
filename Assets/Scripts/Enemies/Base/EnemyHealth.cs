using System;
using UnityEngine;
using Son.Economy;

[RequireComponent(typeof(EnemyGoldDrop))]
[RequireComponent(typeof(EnemyExpDropTest))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float currentHealth;

    private EnemyAI enemyAI;
    private EnemyGoldDrop goldDrop;
    private EnemyExpDropTest expDrop;
    private float maxHealth;
    private bool isDead = false;

    // Events
    public Action onDie;
    public Action onHit;

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        maxHealth = enemyAI != null ? enemyAI.stats.maxHealth : 100f;

        currentHealth = maxHealth;

        goldDrop = GetComponent<EnemyGoldDrop>();
        expDrop = GetComponent<EnemyExpDropTest>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0 || isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > 0)
        {
            onHit?.Invoke();
            enemyAI.ApplyStun();
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        enemyAI.StopAI();
        onDie?.Invoke();

        // goldDrop?.SetRewards(enemyAI.stats.minGold, enemyAI.stats.maxGold);
        // expDrop?.SetExpAmount(enemyAI.stats.expAmount);

        // goldDrop?.OnEnemyKilled();
        // expDrop?.OnEnemyKilled();
    }

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }
}
