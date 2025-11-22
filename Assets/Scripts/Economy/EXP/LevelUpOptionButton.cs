using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Son.Economy
{
    [RequireComponent(typeof(Button))]
    public class LevelUpOptionButton : MonoBehaviour
    {
        [Header("UI Bind")]
        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text descriptionText;

        [HideInInspector] public LevelUpOption_StatBonus option;
        [HideInInspector] public LevelUpPanel parentPanel;

        private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _btn.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);
        }

        public void Setup(LevelUpOption_StatBonus config, LevelUpPanel panel)
        {
            option = config;
            parentPanel = panel;

            if (iconImage) iconImage.sprite = config.icon;
            if (nameText) nameText.text = config.displayName + " +" + config.amount;
            if (descriptionText) descriptionText.text = config.description;
        }

        private void OnClick()
        {
            if (option == null || parentPanel == null) return;

            option.ApplyEffect();
            parentPanel.OnOptionChosen(option);
        }
    }
}
