using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private EnemyAI currentBoss;
    private EnemyHealth bossHealth;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentBoss != null && bossHealth != null)
        {
            float maxHealth = bossHealth.GetMaxHealth();
            float currentHealth = bossHealth.GetCurrentHealth();

            if (maxHealth > 0)
                _slider.value = currentHealth / maxHealth;
        }
    }

    public void HandleBossSpawned(EnemyAI boss)
    {
        if (boss == null) return;

        currentBoss = boss;
        bossHealth = boss.GetComponent<EnemyHealth>();

        if (bossHealth != null)
        {
            gameObject.SetActive(true);

            bossHealth.onHealthChanged += UpdateUI;
            bossHealth.OnDeath += HandleBossDied;

            UpdateUI(bossHealth.GetCurrentHealth(), bossHealth.GetMaxHealth());
        }
    }


    private void HandleBossDied()
    {
        if (bossHealth != null)
        {
            bossHealth.onHealthChanged -= UpdateUI;
            bossHealth.OnDeath -= HandleBossDied;
        }

        currentBoss = null;
        bossHealth = null;
        gameObject.SetActive(false);
    }

    private void UpdateUI(float current, float max)
    {
        if (max > 0)
            _slider.value = current / max;
    }
}
