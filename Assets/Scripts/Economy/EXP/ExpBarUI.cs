using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Son.Economy
{
    /// <summary>
    /// Gắn lên UI thanh EXP.
    /// </summary>
    public class ExpBarUI : MonoBehaviour
    {
        [Header("UI Bind")]
        public Slider expSlider;        // Min=0, Max=1
        public TMP_Text levelText;      // Hiện level hiện tại
        public TMP_Text expText;        // "cur / next" (optional)

        private PlayerExperienceManager _exp;

        private void OnEnable()
        {
            _exp = PlayerExperienceManager.Instance;
            if (_exp == null)
            {
                Debug.LogError("[ExpBarUI] PlayerExperienceManager.Instance == null");
                return;
            }

            _exp.OnExpChanged += OnExpChanged;

            // Sync ngay ban đầu
            OnExpChanged(_exp.currentLevel, _exp.currentExp, _exp.ExpToNextLevel);
        }

        private void OnDisable()
        {
            if (_exp != null)
                _exp.OnExpChanged -= OnExpChanged;
        }

        private void OnExpChanged(int level, int curExp, int expToNext)
        {
            float normalized = expToNext > 0 ? (float)curExp / expToNext : 0f;
            float previousLevel = level - 1f;

            if (expSlider != null)
            {
                expSlider.minValue = 0f;
                expSlider.maxValue = 1f;
                expSlider.value = normalized;
            }

            if (levelText != null)
                levelText.text = level.ToString();

            if (expText != null)
                expText.text = previousLevel.ToString();
        }
    }
}
