using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Son.Economy
{
    [RequireComponent(typeof(Button))]
    public class UnitShopButton : MonoBehaviour
    {
        [Header("Bind")]
        public UnitShopItemConfig item;
        public Image iconImage;
        public TMP_Text nameText;
        public TMP_Text costText;
        public TMP_Text currencyText;
        public TMP_Text feedbackText; // optional

        private Button _btn;
        private UnitPurchaseService _service;
        private WalletManager _wallet;

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _service = UnitPurchaseService.Instance;
            _wallet = WalletManager.Instance;

            if (_wallet != null)
            {
                _wallet.OnCurrencyChanged += OnWalletCurrencyChanged;
                Debug.Log($"[UnitShopButton:{name}] Đã subscribe OnCurrencyChanged.");
            }
            else
            {
                Debug.LogWarning($"[UnitShopButton:{name}] _wallet == null trong OnEnable.");
            }

            if (_service == null)
            {
                Debug.LogWarning($"[UnitShopButton:{name}] UnitPurchaseService.Instance == null trong OnEnable.");
            }

            Refresh();
            _btn.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClick);

            if (_wallet != null)
            {
                _wallet.OnCurrencyChanged -= OnWalletCurrencyChanged;
                Debug.Log($"[UnitShopButton:{name}] Unsubscribe OnCurrencyChanged.");
            }
        }

        private void OnClick()
        {
            if (item == null)
            {
                Debug.LogError($"[UnitShopButton:{name}] OnClick nhưng item == null.");
                return;
            }

            if (_service == null)
            {
                Debug.LogError($"[UnitShopButton:{name}] OnClick nhưng _service == null.");
                ShowFeedback("Thiếu UnitPurchaseService");
                return;
            }

            Debug.Log($"[UnitShopButton:{name}] OnClick → yêu cầu mua unit {item.id} với cost={item.cost} {item.currency}");

            if (!_service.TryBuyAndSpawn(item, out var err))
            {
                Debug.Log($"[UnitShopButton:{name}] Mua thất bại: {err}");
                if (!string.IsNullOrEmpty(err))
                    ShowFeedback(err);
            }
            else
            {
                Debug.Log($"[UnitShopButton:{name}] Mua thành công unit {item.id}.");
                ShowFeedback("Đã gọi lính!");
                Refresh();
            }
        }

        private void Refresh()
        {
            if (item == null)
            {
                Debug.LogWarning($"[UnitShopButton:{name}] Refresh nhưng item == null.");
                SetInteractable(false);
                return;
            }

            if (iconImage) iconImage.sprite = item.icon;
            if (nameText) nameText.text = item.displayName;
            if (costText) costText.text = item.cost.ToString();
            if (currencyText) currencyText.text = item.currency.ToString();

            bool enough = _wallet != null && _wallet.HasEnough(item.currency, item.cost);
            float cd = _service != null ? _service.GetCooldownRemain(item) : 0f;
            bool onCd = cd > 0f;

            SetInteractable(enough && !onCd);

            if (onCd)
                ShowFeedback($"CD: {cd:0.0}s");
            else if (!enough)
                ShowFeedback("Không đủ tiền");
            else
                ClearFeedback();

            Debug.Log($"[UnitShopButton:{name}] Refresh → enough={enough}, cd={cd:0.0}, interactable={_btn.interactable}");
        }

        private void SetInteractable(bool value)
        {
            if (_btn) _btn.interactable = value;
        }

        private void OnWalletCurrencyChanged(CurrencyType type, int newBalance)
        {
            if (item != null && type == item.currency)
            {
                Debug.Log($"[UnitShopButton:{name}] OnWalletCurrencyChanged {type}={newBalance} → Refresh()");
                Refresh();
            }
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
