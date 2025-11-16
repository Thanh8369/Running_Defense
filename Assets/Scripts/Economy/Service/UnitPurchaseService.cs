using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    public class UnitPurchaseService : MonoBehaviour
    {
        public static UnitPurchaseService Instance { get; private set; }

        [Header("Deps")]
        public WalletManager wallet;
        [Tooltip("Vị trí spawn lính khi mua.")]
        public Transform spawnPoint;
        [Tooltip("Parent để chứa các lính spawn ra (optional).")]
        public Transform unitsParent;

        private readonly Dictionary<string, float> _cooldownRemain = new();
        private readonly Dictionary<string, int> _aliveCount = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[UnitPurchaseService] Duplicate instance, destroying this one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[UnitPurchaseService] Awake, set Instance & DontDestroyOnLoad.");
        }

        private void Start()
        {
            if (wallet == null) wallet = WalletManager.Instance;

            if (wallet == null)
                Debug.LogError("[UnitPurchaseService] Missing WalletManager (wallet == null).");
            else
                Debug.Log("[UnitPurchaseService] WalletManager linked OK.");

            if (spawnPoint == null)
                Debug.LogWarning("[UnitPurchaseService] spawnPoint chưa được gán. Sẽ spawn tại (0,0,0).");
        }

        private void Update()
        {
            if (_cooldownRemain.Count == 0) return;

            var keys = new List<string>(_cooldownRemain.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                var id = keys[i];
                float before = _cooldownRemain[id];
                _cooldownRemain[id] -= Time.deltaTime;
                if (_cooldownRemain[id] <= 0f)
                    _cooldownRemain[id] = 0f;

                // Debug nhẹ nếu muốn xem CD giảm
                // Debug.Log($"[UnitPurchaseService] CD {id}: {before:0.0} -> {_cooldownRemain[id]:0.0}");
            }
        }

        public void SetSpawnPoint(Transform point)
        {
            spawnPoint = point;

            if (point == null)
            {
                Debug.LogWarning("[UnitPurchaseService] SpawnPoint set to NULL. " +
                                 "Unit sẽ spawn tại (0,0,0) cho tới khi được set lại.");
            }
            else
            {
                Debug.Log($"[UnitPurchaseService] SpawnPoint set to {point.name}");
            }
        }

        public bool TryBuyAndSpawn(UnitShopItemConfig item, out string error)
        {
            error = string.Empty;

            if (item == null)
            {
                error = "Item null";
                Debug.LogError("[UnitPurchaseService] TryBuyAndSpawn thất bại: item null.");
                return false;
            }

            Debug.Log($"[UnitPurchaseService] TryBuyAndSpawn request: {item.id} ({item.displayName}), cost={item.cost} {item.currency}");

            if (wallet == null)
            {
                error = "Thiếu WalletManager";
                Debug.LogError("[UnitPurchaseService] wallet == null khi mua lính.");
                return false;
            }

            // Check cooldown
            if (item.cooldown > 0f &&
                _cooldownRemain.TryGetValue(item.id, out float cd) &&
                cd > 0f)
            {
                error = $"Đang cooldown ({cd:0.0}s)";
                Debug.Log($"[UnitPurchaseService] Không thể mua {item.id}: cooldown còn {cd:0.0}s");
                return false;
            }

            // Check max alive
            if (item.maxAliveAtOnce > 0 &&
                _aliveCount.TryGetValue(item.id, out int alive) &&
                alive >= item.maxAliveAtOnce)
            {
                error = "Đã đạt số lượng lính tối đa trên sân.";
                Debug.Log($"[UnitPurchaseService] Không thể mua {item.id}: alive={alive} / max={item.maxAliveAtOnce}");
                return false;
            }

            // Check tiền
            if (!wallet.HasEnough(item.currency, item.cost))
            {
                int current = wallet.GetBalance(item.currency);
                error = $"Không đủ {item.currency} (có {current}, cần {item.cost})";
                Debug.Log($"[UnitPurchaseService] Không đủ tiền mua {item.id}: {current}/{item.cost} {item.currency}");
                return false;
            }

            // Trừ tiền
            bool spendOk = wallet.SpendCurrency(item.currency, item.cost, $"BuyUnit:{item.id}");
            Debug.Log($"[UnitPurchaseService] SpendCurrency result = {spendOk} cho {item.id}");

            if (!spendOk)
            {
                error = "Trừ tiền thất bại.";
                return false;
            }

            // Spawn lính
            if (item.unitPrefab == null)
            {
                error = "Chưa gán prefab lính.";
                Debug.LogError($"[UnitPurchaseService] unitPrefab null cho item {item.id}");
                return false;
            }

            Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
            Transform parent = unitsParent != null ? unitsParent : null;

            GameObject unit = Instantiate(item.unitPrefab, pos, rot, parent);
            Debug.Log($"[UnitPurchaseService] Spawned unit {item.id} tại {pos}");

            // Đếm alive
            if (!_aliveCount.ContainsKey(item.id)) _aliveCount[item.id] = 0;
            _aliveCount[item.id]++;
            Debug.Log($"[UnitPurchaseService] Alive count {item.id} = {_aliveCount[item.id]}");

            // Gắn notifier để biết khi unit chết
            var notifier = unit.AddComponent<UnitLifetimeNotifier>();
            notifier.itemId = item.id;
            notifier.service = this;

            // Set cooldown
            if (item.cooldown > 0f)
            {
                _cooldownRemain[item.id] = item.cooldown;
                Debug.Log($"[UnitPurchaseService] Set cooldown {item.id} = {item.cooldown}s");
            }

            return true;
        }

        public void NotifyUnitDied(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            if (_aliveCount.TryGetValue(itemId, out int alive))
            {
                int before = alive;
                alive = Mathf.Max(0, alive - 1);
                _aliveCount[itemId] = alive;
                Debug.Log($"[UnitPurchaseService] Unit died: {itemId}, alive {before} -> {alive}");
            }
        }

        public float GetCooldownRemain(UnitShopItemConfig item)
        {
            if (item == null || item.cooldown <= 0f) return 0f;
            if (_cooldownRemain.TryGetValue(item.id, out float cd))
                return Mathf.Max(0f, cd);
            return 0f;
        }
    }
}
