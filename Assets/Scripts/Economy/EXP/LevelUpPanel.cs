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
            Debug.Log($"[LevelUpPanel] LEVEL UP {newLevel} → Hiện panel chọn nâng cấp.");
            ShowPanel();
        }

        private void ShowPanel()
        {
            if (panelRoot == null)
            {
                Debug.LogError("[LevelUpPanel] panelRoot == null");
                return;
            }

            // Pause game
            _prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            // Chọn 3 option ngẫu nhiên
            var chosen = PickRandomOptions(3);

            // Bind lên button
            if (optionButton1 != null) optionButton1.Setup(chosen.Count > 0 ? chosen[0] : null, this);
            if (optionButton2 != null) optionButton2.Setup(chosen.Count > 1 ? chosen[1] : null, this);
            if (optionButton3 != null) optionButton3.Setup(chosen.Count > 2 ? chosen[2] : null, this);

            panelRoot.SetActive(true);
        }

        public void OnOptionChosen(LevelUpOptionConfig option)
        {
            Debug.Log($"[LevelUpPanel] Player đã chọn option: {option?.id} - {option?.displayName}");

            // Tắt panel + resume game
            if (panelRoot != null)
                panelRoot.SetActive(false);

            Time.timeScale = _prevTimeScale;
        }

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
