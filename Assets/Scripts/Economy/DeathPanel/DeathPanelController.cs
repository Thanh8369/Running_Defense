using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Son.Economy
{
    public class DeathPanelController : MonoBehaviour
    {
        public static DeathPanelController Instance { get; private set; }

        [Header("Root Panel")]
        public GameObject panelRoot;

        [Header("UI Buttons")]
        public Button btnContinueGem;     // nút xanh 12 Gem
        public Button btnContinueAd;      // nút vàng FREE
        public Button btnClose;           // nút X

        [Header("UI Text")]
        public TextMeshProUGUI gemCostText;

        [Header("Chi phí hồi sinh")]
        public int gemCost = 12;

        [Header("Event Hook")]
        public UnityEvent onContinue;   // gọi khi revive
        public UnityEvent onQuit;       // gọi khi quit

        private float _prevTimeScale = 1f;
        private bool _isOpen = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (panelRoot != null)
                panelRoot.SetActive(false);

            if (btnContinueGem != null)
                btnContinueGem.onClick.AddListener(OnClickContinueGem);

            if (btnContinueAd != null)
                btnContinueAd.onClick.AddListener(OnClickContinueAd);

            if (btnClose != null)
                btnClose.onClick.AddListener(OnClickClose);
        }

        // ---------------- PUBLIC API ----------------

        /// <summary>
        /// Gọi hàm này khi Player chết
        /// </summary>
        public void Show()
        {
            if (_isOpen) return;

            _isOpen = true;

            if (panelRoot != null)
                panelRoot.SetActive(true);

            if (gemCostText != null)
                gemCostText.text = gemCost.ToString();

            _prevTimeScale = Time.timeScale;
            Time.timeScale = 0f; // pause game
        }

        /// <summary>
        /// Ẩn panel và resume game
        /// </summary>
        public void Hide()
        {
            if (!_isOpen) return;

            _isOpen = false;

            if (panelRoot != null)
                panelRoot.SetActive(false);

            Time.timeScale = _prevTimeScale;
        }

        // ---------------- BUTTON EVENTS ----------------

        private void OnClickContinueGem()
        {
            // Spend Gem using WalletManager
            bool paid = WalletManager.Instance.SpendCurrency(CurrencyType.Gem, gemCost, "Revive by Gem");

            if (!paid)
            {
                Debug.Log("[DeathPanel] Không đủ Gem để hồi sinh.");
                // TODO: hiện popup "Not enough gems"
                return;
            }

            ContinueGame();
        }

        private void OnClickContinueAd()
        {
            Debug.Log("[DeathPanel] Giả lập xem Ads thành công → cho revive.");
            // TODO: tích hợp Ads thật sau
            ContinueGame();
        }

        private void OnClickClose()
        {
            Debug.Log("[DeathPanel] Player chọn Quit.");
            Hide();
            onQuit?.Invoke();
        }

        // ---------------- INTERNAL LOGIC ----------------

        private void ContinueGame()
        {
            Debug.Log("[DeathPanel] Continue Game");

            Hide();

            // Gọi logic revive bạn gán trong Inspector
            onContinue?.Invoke();
        }
    }
}
