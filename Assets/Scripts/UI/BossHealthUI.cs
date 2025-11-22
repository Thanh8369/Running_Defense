using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    private LizardWarriorAI _currentBoss;
    private EnemyHealth _bossHealth;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_currentBoss != null && _bossHealth != null)
        {
            float maxHealth = _bossHealth.GetMaxHealth();
            float currentHealth = _bossHealth.GetCurrentHealth();

            if (maxHealth > 0)
                _slider.value = currentHealth / maxHealth;
        }
    }

    public void HandleBossSpawned(LizardWarriorAI boss)
    {
        if(boss == null) return;

        _currentBoss = boss;
        _bossHealth = boss.GetComponent<EnemyHealth>();

        if (_bossHealth != null)
        {
            gameObject.SetActive(true);

            _bossHealth.onHealthChanged += UpdateUI;
            _bossHealth.onDie += HandleBossDied;

            UpdateUI(_bossHealth.GetCurrentHealth(), _bossHealth.GetMaxHealth());
        }
    }

    private void HandleBossDied()
    {
        if (_bossHealth != null)
        {
            _bossHealth.onHealthChanged -= UpdateUI;
            _bossHealth.onDie -= HandleBossDied;
        }

        _currentBoss = null;
        _bossHealth = null;
        gameObject.SetActive(false);
    }

    private void UpdateUI(float current, float max)
    {
        if (max > 0)
            _slider.value = current / max;
    }
}
