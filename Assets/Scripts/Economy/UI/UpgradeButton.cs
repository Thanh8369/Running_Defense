using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Son.Economy
{
    [RequireComponent(typeof(Button))]
    public class UpgradeButton : MonoBehaviour
    {
        [Header("Bind")]
        public ShopItemConfig item;
        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text levelText;
        public TMP_Text costText;
        public TMP_Text currencyText;
        public TMP_Text feedbackText; // optional: hiện lỗi ngắn

        private Button _btn;
        private PurchaseService _purchase;
        private ShopState _state;
        private WalletManager _wallet;

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            // Lấy singleton thay vì FindObjectOfType (không còn warning obsolete)
            _purchase = PurchaseService.Instance;
            _state = ShopState.Instance;
            _wallet = WalletManager.Instance;

            // Đăng ký event đúng tên: OnCurrencyChanged (không phải OnBalanceChanged)
            if (_wallet != null) _wallet.OnCurrencyChanged += OnWalletCurrencyChanged;
            if (_state != null) _state.OnLevelChanged += OnLevelChanged;
            if (_purchase != null)
            {
                _purchase.OnPurchased += OnPurchased;
                _purchase.OnPurchaseFailed += OnPurchaseFailed;
            }

            Refresh();
            _btn.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);

            if (_wallet != null) _wallet.OnCurrencyChanged -= OnWalletCurrencyChanged;
            if (_state != null) _state.OnLevelChanged -= OnLevelChanged;
            if (_purchase != null)
            {
                _purchase.OnPurchased -= OnPurchased;
                _purchase.OnPurchaseFailed -= OnPurchaseFailed;
            }
        }

        private void OnClick()
        {
            if (_purchase == null || item == null) return;

            _ = _purchase.TryPurchase(item, out var err);
            if (!string.IsNullOrEmpty(err))
                ShowFeedback(err);
        }

        private void Refresh()
        {
            if (item == null)
            {
                SetInteractable(false);
                return;
            }

            // Icon + tên
            if (iconImage) iconImage.sprite = item.icon;
            if (nameText) nameText.text = item.displayName;

            // Level hiện tại
            int lv = _state != null ? _state.GetLevel(item.id) : 0;
            if (levelText) levelText.text = $"Lv {lv}/{item.maxLevel}";

            // Giá hiện tại
            int cost = item.GetCost(lv);
            if (costText) costText.text = cost.ToString();
            if (currencyText) currencyText.text = item.currency.ToString();

            bool maxed = lv >= item.maxLevel;
            bool enough = _wallet != null && _wallet.HasEnough(item.currency, cost);

            SetInteractable(!maxed && enough);

            // Nếu thiếu tiền thì làm mờ nút
            if (!maxed && !enough && _btn.targetGraphic != null)
                _btn.targetGraphic.canvasRenderer.SetAlpha(0.6f);
            else if (_btn.targetGraphic != null)
                _btn.targetGraphic.canvasRenderer.SetAlpha(1f);

            if (maxed) ShowFeedback("Max level");
            else ClearFeedback();
        }

        private void SetInteractable(bool value)
        {
            if (_btn) _btn.interactable = value;
        }

        /// <summary>
        /// Event từ WalletManager: OnCurrencyChanged(CurrencyType type, int newBalance)
        /// </summary>
        private void OnWalletCurrencyChanged(CurrencyType type, int newBalance)
        {
            // Chỉ refresh nếu loại tiền trùng với item này dùng
            if (item != null && type == item.currency)
                Refresh();
        }

        private void OnLevelChanged(string itemId, int _)
        {
            if (item != null && item.id == itemId)
                Refresh();
        }

        private void OnPurchased(ShopItemConfig purchasedItem, int _)
        {
            if (item != null && item == purchasedItem)
            {
                ShowFeedback("Đã nâng cấp!");
                Refresh();
            }
        }

        private void OnPurchaseFailed(ShopItemConfig failedItem, string reason)
        {
            if (item != null && item == failedItem)
                ShowFeedback(reason);
        }

        private void ShowFeedback(string msg)
        {
            if (feedbackText) feedbackText.text = msg;
        }

        private void ClearFeedback()
        {
            if (feedbackText) feedbackText.text = "";
        }
    }
}
