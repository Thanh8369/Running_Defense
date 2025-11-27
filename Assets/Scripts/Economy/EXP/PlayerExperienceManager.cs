using System;
using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Quản lý EXP và Level của người chơi.
    /// - Cộng EXP.
    /// - Khi đủ -> Level Up, trừ exp, tăng level, bắn event.
    /// </summary>
    public class PlayerExperienceManager : MonoBehaviour
    {
        public static PlayerExperienceManager Instance { get; private set; }

        [Header("Cấu hình Level")]
        [Tooltip("Level hiện tại (bắt đầu).")]
        public int currentLevel = 1;

        [Tooltip("EXP hiện tại trong level hiện tại.")]
        public int currentExp = 0;

        [Tooltip("EXP cần cho level 1 -> 2.")]
        public int baseExpToLevelUp = 100;

        [Tooltip("Mỗi level tăng thêm bao nhiêu % EXP cần thiết (0.2 = +20%).")]
        [Range(0f, 1f)]
        public float expGrowthPerLevel = 0.2f;

        public bool canGainExp = true;

        /// <summary>
        /// EXP yêu cầu cho level hiện tại -> level + 1.
        /// </summary>
        public int ExpToNextLevel
        {
            get
            {
                // ví dụ: base * (1 + growth * (level - 1))
                float factor = 1f + expGrowthPerLevel * (currentLevel - 1);
                return Mathf.Max(1, Mathf.RoundToInt(baseExpToLevelUp * factor));
            }
        }

        // Event cho UI & game logic
        public event Action<int, int, int> OnExpChanged;  // (level, currentExp, expToNext)
        public event Action<int> OnLevelUp;               // (newLevel)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Bắn event lần đầu cho UI sync
            OnExpChanged?.Invoke(currentLevel, currentExp, ExpToNextLevel);
        }

        /// <summary>
        /// Cộng EXP. Có thể lên nhiều level nếu exp dư nhiều.
        /// </summary>
        public void AddExp(int amount)
        {
            if (amount <= 0) return;

            // KHÓA NHẬN EXP KHI CHẾT HOẶC ĐANG REVIVE
            if (!canGainExp)
            {
                Debug.Log("[PlayerExperience] Đang chết / revive → không nhận EXP.");
                return;
            }

            int expToNext = ExpToNextLevel;
            currentExp += amount;
            Debug.Log($"[PlayerExperience] AddExp {amount}. EXP={currentExp}/{expToNext} (Lv {currentLevel})");

            // Loop nếu dư exp
            while (currentExp >= expToNext)
            {
                currentExp -= expToNext;
                currentLevel++;
                Debug.Log($"[PlayerExperience] LEVEL UP! New Level = {currentLevel}");

                OnLevelUp?.Invoke(currentLevel);

                expToNext = ExpToNextLevel;
                // Nếu level tăng làm expToNext nhỏ hơn 1 (hiếm) thì dừng tránh loop vô hạn
                if (expToNext <= 0) break;
            }

            OnExpChanged?.Invoke(currentLevel, currentExp, ExpToNextLevel);
        }
    }
}
