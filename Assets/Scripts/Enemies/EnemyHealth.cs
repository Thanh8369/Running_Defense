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
        if (Input.GetKeyDown(KeyCode.K))
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

        // 1) Dừng AI, gọi event chết
        enemyAI.StopAI();
        onDie?.Invoke();

        int goldGain = 0;

        //// 2) Gán reward từ stats
        //if (enemyAI != null)
        //{
        //    if (goldDrop != null)
        //        goldDrop.DropGoldAndReturnAmount(enemyAI.stats.minGold, enemyAI.stats.maxGold);

        //    if (expDrop != null)
        //        expDrop.SetExpAmount(enemyAI.stats.expAmount);
        //}

        // 3) Cộng Gold và lấy số Gold đã rơi
        if (goldDrop != null)
        {
            goldGain = goldDrop.DropGoldAndReturnAmount();
        }

        // 4) Cộng Exp
        if (expDrop != null)
        {
            expDrop.OnEnemyKilled();
        }

        // 5) Hiển thị popup Gold (coin + text)
        if (goldGain > 0)
        {
            if (GoldPopupSpawner.Instance != null)
            {
                GoldPopupSpawner.Instance.SpawnGoldPopup(transform.position, goldGain);
                Debug.Log("[EnemyHealth] Spawn gold popup: " + goldGain);
            }
            else
            {
                Debug.LogWarning("[EnemyHealth] GoldPopupSpawner.Instance == null, không thể spawn popup.");
            }
        }

        // 6) Huỷ enemy (sau này nếu có animation chết thì có thể chuyển sang OnDieAnimationEnd)
        Destroy(gameObject);
    }

    public void OnDieAnimationEnd()
    {
        Destroy(gameObject);
    }
}
