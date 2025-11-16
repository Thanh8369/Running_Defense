using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    public class LevelUpPanel : MonoBehaviour
    {
        [Header("Root Panel")]
        public GameObject panelRoot;

        [Header("Danh sách lựa chọn có thể xuất hiện")]
        public List<LevelUpOptionConfig> allOptions = new List<LevelUpOptionConfig>();

        [Header("3 nút lựa chọn")]
        public LevelUpOptionButton optionButton1;
        public LevelUpOptionButton optionButton2;
        public LevelUpOptionButton optionButton3;

        [Header("Cấu hình random")]
        public bool allowDuplicateInOneRoll = false;

        private PlayerExperienceManager _exp;
        private float _prevTimeScale = 1f;

        /// <summary>
        /// Số lần LevelUp đang chờ được người chơi chọn (có thể >1 nếu nhảy nhiều level).
        /// </summary>
        private int _pendingLevelUpCount = 0;

        /// <summary>
        /// Panel đang mở hay không.
        /// </summary>
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

        /// <summary>
        /// Được gọi MỖI LẦN level tăng (có thể nhiều lần trong 1 AddExp).
        /// </summary>
        private void OnLevelUp(int newLevel)
        {
            _pendingLevelUpCount++;
            Debug.Log($"[LevelUpPanel] LEVEL UP {newLevel} → pendingLevelUp={_pendingLevelUpCount}");

            // Nếu panel chưa mở thì mở và roll lần đầu.
            if (!_isPanelOpen)
            {
                OpenPanelAndRoll();
            }
            // Nếu panel đang mở sẵn thì không mở lại, chỉ cần chờ người chơi chọn xong lần hiện tại.
        }

        /// <summary>
        /// Mở panel (nếu đang tắt), pause game, và roll option cho LẦN nâng cấp hiện tại.
        /// </summary>
        private void OpenPanelAndRoll()
        {
            if (panelRoot == null)
            {
                Debug.LogError("[LevelUpPanel] panelRoot == null");
                return;
            }

            // Nếu panel chưa mở thì lưu timeScale & pause game + bật panel
            if (!_isPanelOpen)
            {
                _prevTimeScale = Time.timeScale;
                Time.timeScale = 0f;

                panelRoot.SetActive(true);
                _isPanelOpen = true;
            }

            // Mỗi lần gọi hàm này sẽ roll lại 3 option mới cho 1 lần LevelUp.
            var chosen = PickRandomOptions(3);

            if (optionButton1 != null) optionButton1.Setup(chosen.Count > 0 ? chosen[0] : null, this);
            if (optionButton2 != null) optionButton2.Setup(chosen.Count > 1 ? chosen[1] : null, this);
            if (optionButton3 != null) optionButton3.Setup(chosen.Count > 2 ? chosen[2] : null, this);

            Debug.Log($"[LevelUpPanel] OpenPanelAndRoll -> Roll options cho 1 lần LevelUp. pending={_pendingLevelUpCount}");
        }

        /// <summary>
        /// Gọi từ LevelUpOptionButton khi người chơi chọn 1 option.
        /// </summary>
        public void OnOptionChosen(LevelUpOptionConfig option)
        {
            Debug.Log($"[LevelUpPanel] Player đã chọn option: {option?.id} - {option?.displayName}");

            // Đã xử lý xong 1 lần LevelUp
            _pendingLevelUpCount = Mathf.Max(0, _pendingLevelUpCount - 1);

            // Nếu vẫn còn LevelUp đang chờ → roll tiếp option cho lần tiếp theo.
            if (_pendingLevelUpCount > 0)
            {
                Debug.Log($"[LevelUpPanel] Còn {_pendingLevelUpCount} lần LevelUp chờ → roll panel tiếp.");
                OpenPanelAndRoll();
            }
            else
            {
                // Không còn LevelUp chờ → đóng panel & resume game
                if (panelRoot != null)
                    panelRoot.SetActive(false);

                Time.timeScale = _prevTimeScale;
                _isPanelOpen = false;

                Debug.Log("[LevelUpPanel] Hết pending LevelUp → đóng panel, resume game.");
            }
        }

        /// <summary>
        /// Random 3 option từ danh sách allOptions.
        /// </summary>
        private List<LevelUpOptionConfig> PickRandomOptions(int count)
        {
            var result = new List<LevelUpOptionConfig>();

            if (allOptions == null || allOptions.Count == 0)
                return result;

            // clone tạm list để remove nếu không cho trùng
            var pool = new List<LevelUpOptionConfig>(allOptions);

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
