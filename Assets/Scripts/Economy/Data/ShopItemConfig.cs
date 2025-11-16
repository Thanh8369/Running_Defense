using UnityEngine;

namespace Son.Economy
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "Son/Economy/Shop Item", order = 0)]
    public class ShopItemConfig : ScriptableObject
    {
        [Header("Identity")]
        public string id;                           // duy nhất, ví dụ: "atk_speed_1"
        public string displayName;                  // tên hiển thị
        [TextArea] public string description;      // mô tả ngắn

        [Header("Visuals")]
        public Sprite icon;

        [Header("Economy")]
        public CurrencyType currency = CurrencyType.Gold;
        public int baseCost = 100;                 // giá ở level 0
        [Range(1f, 3f)] public float costMultiplier = 1.15f; // hệ số tăng giá
        public int maxLevel = 10;

        /// <summary>
        /// Giá tại level hiện tại (0-based).
        /// </summary>
        public int GetCost(int currentLevel)
        {
            double cost = baseCost * System.Math.Pow(costMultiplier, currentLevel);
            return Mathf.Max(1, Mathf.RoundToInt((float)cost));
        }
    }
}
