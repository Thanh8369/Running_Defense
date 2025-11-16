using UnityEngine;

namespace Son.Economy
{
    [CreateAssetMenu(fileName = "UnitShopItem", menuName = "Son/Economy/Unit Shop Item", order = 0)]
    public class UnitShopItemConfig : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        [TextArea] public string description;

        [Header("Visual")]
        public Sprite icon;

        [Header("Gameplay")]
        public GameObject unitPrefab;       // Prefab lính sẽ spawn
        public CurrencyType currency = CurrencyType.Gold;
        public int cost = 100;

        [Header("Optional hạn chế")]
        public int maxAliveAtOnce = 0;      // 0 = không giới hạn
        public float cooldown = 0f;         // CD giữa 2 lần mua 1 loại lính (nếu muốn)
    }
}
