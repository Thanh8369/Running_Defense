using System;
using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Lưu level của từng item bằng PlayerPrefs (đơn giản, phù hợp Day 7).
    /// Có thể chuyển sang SaveManager(JSON) ở Day 5 nếu muốn đồng bộ.
    /// </summary>
    public class ShopState : MonoBehaviour
    {
        public static ShopState Instance { get; private set; }
        private const string KeyPrefix = "SHOP_LEVEL_"; // SHOP_LEVEL_{id}

        public event Action<string, int> OnLevelChanged; // (itemId, newLevel)

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int GetLevel(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            return PlayerPrefs.GetInt(KeyPrefix + itemId, 0);
        }

        public void SetLevel(string itemId, int level)
        {
            if (string.IsNullOrEmpty(itemId)) return;
            PlayerPrefs.SetInt(KeyPrefix + itemId, Mathf.Max(0, level));
            PlayerPrefs.Save();
            OnLevelChanged?.Invoke(itemId, level);
        }

        public int IncrementLevel(string itemId, int maxLevel)
        {
            var cur = GetLevel(itemId);
            if (cur >= maxLevel) return cur;
            var next = cur + 1;
            SetLevel(itemId, next);
            return next;
        }
    }
}
