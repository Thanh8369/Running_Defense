using UnityEngine;
using Son.Economy;

/// <summary>
/// Nghe TowerLifeController.OnDeath để hiện DeathPanel,
/// và nghe DeathPanel.onContinue để revive Tower.
/// Gắn script này lên chính object Tower.
/// </summary>
public class DeathController : MonoBehaviour
{
    [Header("Tower Life Controller")]
    public TowerLifeController life;

    [Header("Death Panel Controller")]
    public DeathPanelController deathPanel;

    private void Reset()
    {
        if (life == null)
            life = GetComponent<TowerLifeController>();
    }

    private void Awake()
    {
        if (life == null)
            life = GetComponent<TowerLifeController>();

        if (deathPanel == null)
            deathPanel = FindAnyObjectByType<DeathPanelController>();
    }

    private void OnEnable()
    {
        if (life != null)
            life.OnDeath += HandleDeath;

        if (deathPanel != null)
            deathPanel.onContinue.AddListener(ReviveTower);
    }

    private void OnDisable()
    {
        if (life != null)
            life.OnDeath -= HandleDeath;

        if (deathPanel != null)
            deathPanel.onContinue.RemoveListener(ReviveTower);
    }

    /// <summary>
    /// Gọi khi TowerLifeController bắn OnDeath.
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log("[DeathController] Tower chết → hiện Death Panel");
        deathPanel?.Show();
    }

    /// <summary>
    /// Gọi khi bấm nút Revive trên DeathPanel.
    /// </summary>
    private void ReviveTower()
    {
        Debug.Log("[DeathController] Revive tower");

        if (life == null)
        {
            Debug.LogWarning("[DeathController] Không có TowerLifeController để revive!");
            return;
        }

        // Gọi hàm revive chuẩn của Tower
        life.Revive();
    }
}
