using Son.Economy;
using UnityEngine;

public class EnemyExpDropTest : MonoBehaviour
{
    [Header("Cấu hình EXP khi quái chết")]
    public int expAmount = 10;
    public bool logOnDrop = false;

    private bool _hasGivenExp = false;

    /// <summary>
    /// Gọi từ logic chết của Enemy (cùng chỗ bạn gọi EnemyGoldDrop.OnEnemyKilled()).
    /// </summary>
    public void OnEnemyKilled()
    {
        if (_hasGivenExp) return;
        _hasGivenExp = true;

        if (PlayerExperienceManager.Instance == null)
        {
            Debug.LogError("[EnemyExpDrop] PlayerExperienceManager.Instance == null. Không thể cộng EXP.");
            return;
        }

        if (expAmount <= 0) return;

        PlayerExperienceManager.Instance.AddExp(expAmount);

        if (logOnDrop)
            Debug.Log($"[EnemyExpDrop] {gameObject.name} thưởng {expAmount} EXP.");
    }
}
