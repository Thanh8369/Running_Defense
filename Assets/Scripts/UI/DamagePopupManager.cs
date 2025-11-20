using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamagePopup _popupPrefab;

    public static DamagePopupManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Gọi hàm này để tạo popup damage tại vị trí world.
    /// </summary>
    public void ShowDamage(Vector3 worldPosition, float damage)
    {
        if (_popupPrefab == null)
        {
            Debug.LogWarning("[DamagePopupManager] Popup prefab is not assigned.");
            return;
        }

        DamagePopup popupInstance = Instantiate(
            _popupPrefab,
            worldPosition,
            Quaternion.identity,
            transform   // parent vào Canvas / Manager
        );

        popupInstance.Setup(damage);
    }
}
