using UnityEngine;

public class DamagePopupReceiver : MonoBehaviour
{
    [Header("Anchor")]
    [SerializeField] private Transform _popupAnchor;

    private Transform PopupAnchor => _popupAnchor != null ? _popupAnchor : transform;

    /// <summary>
    /// Gọi hàm này để hiển thị popup damage.
    /// </summary>
    public void ShowDamage(float damage, Vector3 hitPoint)
    {
        if (DamagePopupManager.Instance == null)
        {
            Debug.LogWarning("[DamagePopupReceiver] DamagePopupManager.Instance is null.");
            return;
        }

        Vector3 spawnPos = hitPoint == Vector3.zero ? PopupAnchor.position : hitPoint;
        DamagePopupManager.Instance.ShowDamage(spawnPos, damage);
    }

    /// <summary>
    /// Overload: không cần hitPoint, tự lấy vị trí anchor.
    /// </summary>
    public void ShowDamage(float damage)
    {
        ShowDamage(damage, PopupAnchor.position);
    }
}
