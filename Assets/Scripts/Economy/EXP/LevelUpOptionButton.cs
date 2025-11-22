using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Son.Economy
{
    /// <summary>
    /// Gắn lên 1 button option trong LevelUpPanel.
    /// - Hiển thị icon / title / description.
    /// - Khi bấm, gọi lại LevelUpPanel.OnOptionChosen().
    /// </summary>
    public class LevelUpOptionButton : MonoBehaviour
    {
        [Header("UI")]
        public Image iconImage;
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public Button button;

        private LevelUpOptionConfig _option;
        private LevelUpPanel _panel;

        /// <summary>
        /// Gọi từ LevelUpPanel để gán option và panel cha.
        /// </summary>
        public void Setup(LevelUpOptionConfig option, LevelUpPanel panel)
        {
            _option = option;
            _panel = panel;

            if (option == null)
            {
                // Không có option → ẩn nút
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            if (iconImage != null)
                iconImage.sprite = option.icon;

            if (titleText != null)
                titleText.text = option.displayName;

            if (descriptionText != null)
                descriptionText.text = option.description;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (_option == null || _panel == null)
                return;

            _panel.OnOptionChosen(_option);
        }
    }
}
