using UnityEngine;
using UnityEngine.UI;
using TMPro;   // thêm dòng này

namespace Son.Economy
{
    /// <summary>
    /// Controller cho 1 slot lính:
    /// - avatar: hình/con lính
    /// - buttonOn: phần UI khi ĐỦ tiền (chứa Button mua)
    /// - buttonOff: phần UI khi KHÔNG đủ tiền
    ///
    /// Script này nên gắn lên object parent (TroopSlot).
    /// Nút mua thật sự là buyButton (thường nằm trong buttonOn).
    /// </summary>
    public class UnitShopButton : MonoBehaviour
    {
        [Header("Config")]
        public UnitShopItemConfig item;

        [Header("3 UI Object")]
        public GameObject avatar;
        public GameObject buttonOn;
        public GameObject buttonOff;

        [Header("Button mua (thường là Button con của buttonOn)")]
        public Button buyButton;

        [Header("Hiển thị giá tiền")]
        [Tooltip("Text hiển thị cost lấy từ config (VD: 100).")]
        public TMP_Text costText;

        private UnitPurchaseService _service;
        private WalletManager _wallet;

        private void OnEnable()
        {
            _service = UnitPurchaseService.Instance;
            _wallet = WalletManager.Instance;

            if (_wallet != null)
            {
                _wallet.OnCurrencyChanged += OnWalletCurrencyChanged;
                Debug.Log($"[UnitShopButton:{name}] Subscribe OnCurrencyChanged.");
            }
            else
            {
                Debug.LogWarning($"[UnitShopButton:{name}] _wallet == null trong OnEnable.");
            }

            if (_service == null)
            {
                Debug.LogWarning($"[UnitShopButton:{name}] UnitPurchaseService.Instance == null trong OnEnable.");
            }

            if (buyButton != null)
                buyButton.onClick.AddListener(OnClick);

            Refresh();
        }

        private void OnDisable()
        {
            if (buyButton != null)
                buyButton.onClick.RemoveListener(OnClick);

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
                return;
            }

            Debug.Log($"[UnitShopButton:{name}] OnClick → mua unit {item.id} cost={item.cost} {item.currency}");

            if (!_service.TryBuyAndSpawn(item, out var err))
            {
                Debug.Log($"[UnitShopButton:{name}] Mua thất bại: {err}");
            }
            else
            {
                Debug.Log($"[UnitShopButton:{name}] Mua thành công unit {item.id}.");
                Refresh();
            }
        }

        private void Refresh()
        {
            if (item == null)
            {
                Debug.LogWarning($"[UnitShopButton:{name}] Refresh nhưng item == null.");
                SetAvatarActive(false);
                SetButtonStates(false, false, 0f);
                SetInteractable(false);

                // clear text nếu không có item
                if (costText != null)
                    costText.text = "";

                // clear sprite avatar nếu muốn
                if (avatar != null)
                {
                    var img = avatar.GetComponent<Image>();
                    if (img != null) img.sprite = null;
                }

                return;
            }

            // Cập nhật sprite avatar từ config.icon
            if (avatar != null)
            {
                var img = avatar.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = item.icon;
                }
            }

            // Cập nhật text giá từ config
            if (costText != null)
            {
                costText.text = item.cost.ToString();
                // hoặc nếu muốn: costText.text = $"{item.cost} {item.currency}";
            }

            bool enoughMoney = _wallet != null && _wallet.HasEnough(item.currency, item.cost);
            float cd = _service != null ? _service.GetCooldownRemain(item) : 0f;
            bool onCd = cd > 0f;

            bool canBuyNow = enoughMoney && !onCd;

            SetAvatarActive(true);
            SetButtonStates(enoughMoney, onCd, cd);
            SetInteractable(canBuyNow);

            Debug.Log($"[UnitShopButton:{name}] Refresh → enough={enoughMoney}, cd={cd:0.0}, canBuyNow={canBuyNow}");
        }

        private void SetAvatarActive(bool active)
        {
            if (avatar != null)
                avatar.SetActive(active);
        }

        private void SetButtonStates(bool enoughMoney, bool onCd, float cdRemain)
        {
            // Đủ tiền → hiện buttonOn, ẩn buttonOff
            // Không đủ tiền → ẩn buttonOn, hiện buttonOff
            // Cooldown: vẫn hiện buttonOn nhưng SetInteractable(false) để khóa click

            if (buttonOn != null)
                buttonOn.SetActive(enoughMoney);

            if (buttonOff != null)
                buttonOff.SetActive(!enoughMoney);
        }

        private void SetInteractable(bool value)
        {
            if (buyButton != null)
                buyButton.interactable = value;
        }

        private void OnWalletCurrencyChanged(CurrencyType type, int newBalance)
        {
            if (item != null && type == item.currency)
            {
                Debug.Log($"[UnitShopButton:{name}] OnWalletCurrencyChanged {type}={newBalance} → Refresh()");
                Refresh();
            }
        }
    }
}
