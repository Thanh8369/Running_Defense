using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Quản lý các dữ liệu Economy liên quan đến gameplay của scene này.
    /// Chức năng chính: đăng ký spawnPoint cho UnitPurchaseService.
    /// </summary>
    public class EconomyShopManager : MonoBehaviour
    {
        [Header("Spawn Point cho lính mua trong trận")]
        [Tooltip("Kéo Transform spawn point của map hiện tại vào đây.")]
        public Transform unitSpawnPoint;

        private void Start()
        {
            if (UnitPurchaseService.Instance == null)
            {
                Debug.LogError("[EconomyShopManager] UnitPurchaseService.Instance == null. " +
                               "Hãy đảm bảo Bootstrap scene đã load và chứa UnitPurchaseService.");
                return;
            }

            if (unitSpawnPoint == null)
            {
                Debug.LogWarning("[EconomyShopManager] unitSpawnPoint chưa được gán. " +
                                 "Unit sẽ spawn tại (0,0,0).");
            }

            UnitPurchaseService.Instance.SetSpawnPoint(unitSpawnPoint);

            Debug.Log($"[EconomyShopManager] SpawnPoint đã đăng ký: " +
                      $"{(unitSpawnPoint != null ? unitSpawnPoint.name : "NULL → dùng Vector3.zero")}.");
        }
    }
}
