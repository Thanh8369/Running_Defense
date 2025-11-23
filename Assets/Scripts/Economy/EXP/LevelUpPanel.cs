using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Quản lý UI Level Up:
    /// - Lắng nghe sự kiện OnLevelUp từ PlayerExperienceManager.
    /// - Mỗi lần Level Up: random 3 option từ allOptions (có thể gồm Player & Tower).
    /// - Cho phép queue nhiều lần Level Up (pendingLevelUpCount).
    /// - Mỗi option chỉ được CHỌN tối đa maxChosenPerOption lần trong 1 run.
    /// </summary>
    public class LevelUpPanel : MonoBehaviour
    {
        [Header("Root Panel")]
        public GameObject panelRoot;

        [Header("Danh sách lựa chọn có thể xuất hiện")]
        [Tooltip("Kéo các ScriptableObject LevelUpOptionConfig (Player, Tower...) vào đây.")]
        public List<LevelUpOptionConfig> allOptions = new List<LevelUpOptionConfig>();

        [Header("3 nút lựa chọn")]
        public LevelUpOptionButton optionButton1;
        public LevelUpOptionButton optionButton2;
        public LevelUpOptionButton optionButton3;

        [Header("Target stats để áp effect (Player)")]
        [Tooltip("PlayerRunStats để option Player có thể buff. Nếu để trống sẽ tự FindAnyObjectByType.")]
        public PlayerRunStats playerRunStats;

        [Header("Cấu hình random")]
        [Tooltip("Cho phép trùng option trong cùng 1 lần roll hay không.")]
        public bool allowDuplicateInOneRoll = false;

        [Header("Giới hạn số lần CHỌN")]
        [Tooltip("Mỗi option chỉ được CHỌN tối đa bao nhiêu lần trong 1 run.")]
        public int maxChosenPerOption = 5;   // <--- NEW

        private PlayerExperienceManager _exp;
        private float _prevTimeScale = 1f;

        private int _pendingLevelUpCount = 0;
        private bool _isPanelOpen = false;

        // Đếm số lần MỖI OPTION đã được CHỌN (player click).
        private readonly Dictionary<LevelUpOptionConfig, int> _optionChosenCount
            = new Dictionary<LevelUpOptionConfig, int>(); // <--- NEW

        private void Awake()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            // Chuẩn bị dictionary cho tất cả option
            foreach (var opt in allOptions)
            {
                if (opt == null) continue;
                if (!_optionChosenCount.ContainsKey(opt))
                    _optionChosenCount[opt] = 0;
            }
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

            if (!_isPanelOpen && _exp.currentLevel <= 35)
            {
                _prevTimeScale = Time.timeScale;
                Time.timeScale = 0f;

                panelRoot.SetActive(true);
                _isPanelOpen = true;
            }

            List<LevelUpOptionConfig> chosen = PickRandomOptions(3);

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
            Debug.Log($"[LevelUpPanel] Player đã CHỌN option: {option?.id} - {option?.displayName}");

            // Ghi nhận số lần CHỌN của option này
            if (option != null)
            {
                if (!_optionChosenCount.ContainsKey(option))
                    _optionChosenCount[option] = 0;

                _optionChosenCount[option]++;

                Debug.Log($"[LevelUpPanel] Option {option.id} đã được chọn tổng cộng: {_optionChosenCount[option]} lần.");
            }

            // --- APPLY EFFECT ---
            if (option != null)
            {
                // Tự tìm PlayerRunStats nếu chưa gán trong Inspector
                if (playerRunStats == null)
                {
                    playerRunStats = Object.FindAnyObjectByType<PlayerRunStats>();
                }

                // Với option Player: dùng playerRunStats.
                // Với option Tower: bỏ qua playerRunStats và tự xử lý.
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

        /// <summary>
        /// Random ra N option bất kỳ từ allOptions,
        /// nhưng chỉ chọn những option CHƯA bị chọn quá maxChosenPerOption lần.
        /// </summary>
        private List<LevelUpOptionConfig> PickRandomOptions(int count)
        {
            var result = new List<LevelUpOptionConfig>();

            if (allOptions == null || allOptions.Count == 0)
                return result;

            // Đảm bảo tất cả option nằm trong dictionary
            foreach (var opt in allOptions)
            {
                if (opt == null) continue;
                if (!_optionChosenCount.ContainsKey(opt))
                    _optionChosenCount[opt] = 0;
            }

            for (int i = 0; i < count; i++)
            {
                // Tạo pool theo rule:
                // - Chỉ lấy option đã được CHỌN < maxChosenPerOption lần.
                // - Nếu không cho trùng trong cùng 1 roll thì loại option đã nằm trong result.
                var pool = new List<LevelUpOptionConfig>();

                foreach (var opt in allOptions)
                {
                    if (opt == null) continue;

                    int chosenCount = _optionChosenCount[opt];

                    // Đã chọn quá/đủ giới hạn
                    if (chosenCount >= maxChosenPerOption)
                        continue;

                    // Không cho trùng trong cùng 1 lần roll
                    if (!allowDuplicateInOneRoll && result.Contains(opt))
                        continue;

                    pool.Add(opt);
                }

                if (pool.Count == 0)
                {
                    Debug.Log("[LevelUpPanel] Không còn option hợp lệ (đã đạt maxChosenPerOption) → không đủ 3 lựa chọn.");
                    break;
                }

                int index = Random.Range(0, pool.Count);
                var chosenOpt = pool[index];
                result.Add(chosenOpt);
            }

            return result;
        }
    }
}
