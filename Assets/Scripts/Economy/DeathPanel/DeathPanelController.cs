using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;    // thêm để load scene

namespace Son.Economy
{
    public class DeathPanelController : MonoBehaviour
    {
        public static DeathPanelController Instance { get; private set; }

        [Header("Root Panel")]
        public GameObject panelRoot;

        [Header("UI Buttons")]
        public Button btnContinueGem;     // nút dùng GOLD để revive
        public Button btnContinueAd;      // nút trở về Main Menu
        public Button btnClose;           // nút X

        [Header("UI Text")]
        public TextMeshProUGUI gemCostText;  // text hiển thị GOLD cost

        [Header("Chi phí hồi sinh (Gold)")]
        public int gemCost = 12;              // cost ban đầu bằng Gold

        [Header("Scene")]
        [Tooltip("Tên scene main menu để load khi bấm nút về menu.")]
        public string mainMenuSceneName = "MainMenu";

        [Header("Event Hook")]
        public UnityEvent onContinue;   // gọi khi revive
        public UnityEvent onQuit;       // gọi khi quit / về main menu

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
        /// Gọi hàm này khi Tower/Player chết
        /// </summary>
        public void Show()
        {
            if (_isOpen) return;

            _isOpen = true;

            if (panelRoot != null)
                panelRoot.SetActive(true);

            if (gemCostText != null)
                gemCostText.text = gemCost.ToString();   // luôn hiển thị cost hiện tại

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

        // Nút dùng GOLD để hồi sinh
        private void OnClickContinueGem()
        {
            // Spend Gold (không còn dùng Gem)
            bool paid = WalletManager.Instance.SpendCurrency(CurrencyType.Gold, gemCost, "Revive by Gold");

            if (!paid)
            {
                Debug.Log("[DeathPanel] Không đủ Gold để hồi sinh.");
                // TODO: hiện popup "Not enough gold"
                return;
            }

            // Nếu trả được → tiếp tục game (revive tower/player)
            ContinueGame();

            // Sau mỗi lần revive, tăng cost lên gấp đôi
            gemCost *= 2;

            // Cập nhật lại UI nếu panel được mở lại sau này
            if (gemCostText != null)
                gemCostText.text = gemCost.ToString();
        }

        // Nút này giờ dùng để về Main Menu
        private void OnClickContinueAd()
        {
            Debug.Log("[DeathPanel] Player chọn về Main Menu.");

            // Ẩn panel + trả lại timeScale trước
            Hide();

            // Bắn event onQuit nếu có logic custom gắn trong Inspector
            onQuit?.Invoke();

            // Nếu có tên scene, load scene main menu
            if (!string.IsNullOrEmpty(mainMenuSceneName))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }
            else
            {
                Debug.LogWarning("[DeathPanel] mainMenuSceneName đang để trống, không load được Main Menu.");
            }
        }

        private void OnClickClose()
        {
            Debug.Log("[DeathPanel] Player chọn Quit (nút X).");
            Hide();
            onQuit?.Invoke();
        }

        // ---------------- INTERNAL LOGIC ----------------

        private void ContinueGame()
        {
            Debug.Log("[DeathPanel] Continue Game");

            Hide();

            // Gọi logic revive bạn gán trong Inspector (ví dụ: DeathController.ReviveTower)
            onContinue?.Invoke();
        }
    }
}
