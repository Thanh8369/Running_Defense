using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    public class LevelUpPanel : MonoBehaviour
    {
        [Header("Root Panel")]
        public GameObject panelRoot;

        [Header("Danh sách lựa chọn có thể xuất hiện")]
        public List<LevelUpOption_StatBonus> allOptions = new List<LevelUpOption_StatBonus>();

        [Header("3 nút lựa chọn")]
        public LevelUpOptionButton optionButton1;
        public LevelUpOptionButton optionButton2;
        public LevelUpOptionButton optionButton3;

        [Header("Target stats để áp effect")]
        public PlayerRunStats playerRunStats;

        [Header("Cấu hình random")]
        public bool allowDuplicateInOneRoll = false;

        private PlayerExperienceManager _exp;
        private float _prevTimeScale = 1f;

        private int _pendingLevelUpCount = 0;
        private bool _isPanelOpen = false;

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            _exp = PlayerExperienceManager.Instance;
            if (_exp != null)
            {
                _exp.OnLevelUp += OnLevelUp;
            }
            else
            {
                Debug.LogError("[LevelUpPanel] PlayerExperienceManager.Instance == null");
            }
        }

        private void OnDisable()
        {
            if (_exp != null)
                _exp.OnLevelUp -= OnLevelUp;
        }

        private void OnLevelUp(int newLevel)
        {
            _pendingLevelUpCount++;
            Debug.Log($"[LevelUpPanel] LEVEL UP {newLevel} → pendingLevelUp={_pendingLevelUpCount}");

            if (!_isPanelOpen)
            {
                OpenPanelAndRoll();
            }
        }

        private void OpenPanelAndRoll()
        {
            if (panelRoot == null)
            {
                Debug.LogError("[LevelUpPanel] panelRoot == null");
                return;
            }

            if (!_isPanelOpen)
            {
                _prevTimeScale = Time.timeScale;
                Time.timeScale = 0f;

                panelRoot.SetActive(true);
                _isPanelOpen = true;
            }

            var chosen = PickRandomOptions(3);

            if (optionButton1 != null) optionButton1.Setup(chosen.Count > 0 ? chosen[0] : null, this);
            if (optionButton2 != null) optionButton2.Setup(chosen.Count > 1 ? chosen[1] : null, this);
            if (optionButton3 != null) optionButton3.Setup(chosen.Count > 2 ? chosen[2] : null, this);

            Debug.Log($"[LevelUpPanel] Roll options cho 1 lần LevelUp. pending={_pendingLevelUpCount}");
        }

        /// <summary>
        /// Gọi từ LevelUpOptionButton khi người chơi chọn 1 option.
        /// </summary>
        public void OnOptionChosen(LevelUpOptionConfig option)
        {
            Debug.Log($"[LevelUpPanel] Player đã chọn option: {option?.id} - {option?.displayName}");

            // --- APPLY EFFECT ---
            if (option != null)
            {
                // Tự tìm PlayerRunStats nếu chưa gán trong Inspector
                if (playerRunStats == null)
                {
                    playerRunStats = FindAnyObjectByType<PlayerRunStats>();
                }

                option.ApplyEffect(playerRunStats);
            }
            // --------------------

            // Đã xử lý xong 1 lần LevelUp
            _pendingLevelUpCount = Mathf.Max(0, _pendingLevelUpCount - 1);

            if (_pendingLevelUpCount > 0)
            {
                Debug.Log($"[LevelUpPanel] Còn {_pendingLevelUpCount} lần LevelUp chờ → roll panel tiếp.");
                OpenPanelAndRoll();
            }
            else
            {
                if (panelRoot != null)
                    panelRoot.SetActive(false);

                Time.timeScale = _prevTimeScale;
                _isPanelOpen = false;

                Debug.Log("[LevelUpPanel] Hết pending LevelUp → đóng panel, resume game.");
            }
        }

        private List<LevelUpOption_StatBonus> PickRandomOptions(int count)
        {
            var result = new List<LevelUpOption_StatBonus>();

            if (allOptions == null || allOptions.Count == 0)
                return result;

            var pool = new List<LevelUpOption_StatBonus>(allOptions);

            for (int i = 0; i < count; i++)
            {
                if (pool.Count == 0) break;

                int index = Random.Range(0, pool.Count);
                var opt = pool[index];
                result.Add(opt);

                if (!allowDuplicateInOneRoll)
                    pool.RemoveAt(index);
            }

            return result;
        }
    }
}
