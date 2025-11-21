using UnityEngine;
using Son.Economy;   // để dùng DeathPanelController

/// <summary>
/// Nghe sự kiện chết từ Health và hiện Death Panel.
/// Đồng thời nghe sự kiện onContinue từ DeathPanel để hồi full máu tower.
/// Gắn script này lên Tower (object có Health của tower).
/// </summary>
public class DeathController : MonoBehaviour
{
    [Header("Tham chiếu tới Health của Tower")]
    public Health health;

    [Header("Death Panel Controller (nếu để trống sẽ tự tìm trong scene)")]
    public DeathPanelController deathPanel;

    private void Reset()
    {
        // Tự lấy Health cùng object nếu quên gán
        if (health == null)
            health = GetComponent<Health>();
    }

    private void Awake()
    {
        // Nếu chưa gán DeathPanel trong Inspector thì thử tìm trong scene
        if (deathPanel == null)
        {
            deathPanel = FindAnyObjectByType<DeathPanelController>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            // đăng ký lắng nghe event chết (HP về 0)
            health.OnDeath += HandleDeath;
        }

        if (deathPanel != null)
        {
            // đăng ký lắng nghe nút Continue (Gem + Ad)
            deathPanel.onContinue.AddListener(ReviveTower);
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDeath -= HandleDeath;
        }

        if (deathPanel != null)
        {
            deathPanel.onContinue.RemoveListener(ReviveTower);
        }
    }

    /// <summary>
    /// Gọi khi Health bắn event OnDeath (HP về 0)
    /// </summary>
    private void HandleDeath()
    {
        Debug.Log("[DeathController] Tower chết → hiện Death Panel");

        if (deathPanel != null)
        {
            deathPanel.Show();
        }
        else
        {
            Debug.LogWarning("[DeathController] Không tìm thấy DeathPanelController trong scene!");
        }
    }

    /// <summary>
    /// Gọi khi bấm Continue Gem hoặc Continue Ad.
    /// Chỉ tower này được hồi full máu.
    /// </summary>
    private void ReviveTower()
    {
        if (health == null)
        {
            Debug.LogWarning("[DeathController] Không có Health để revive!");
            return;
        }

        Debug.Log("[DeathController] Revive tower → full máu");
        health.ReviveToFull();
    }
}
