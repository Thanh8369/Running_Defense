using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;   // THÊM DÒNG NÀY

namespace Son.Economy
{
    /// <summary>
    /// Quản lý UI STAGE CLEAR + thưởng Gold & Gem.
    /// </summary>
    public class StageClearRewardUI : MonoBehaviour
    {
        [Header("Root panel của Stage Clear")]
        public GameObject panelRoot;

        [Header("UI hiển thị Gold")]
        public TextMeshProUGUI goldAmountText;
        public Image goldIconImage;

        [Header("UI hiển thị Gem")]
        public TextMeshProUGUI gemAmountText;

        [Header("UI Stars")]
        public GameObject[] stars;
        public Image gemIconImage;

        [Header("Cấu hình thưởng Gold cơ bản")]
        public int baseGoldReward = 1500;
        public int bonusGoldPerStar = 100;
        public int bonusGoldPerDifficulty = 200;

        [Header("Cấu hình thưởng Gem cơ bản")]
        public int baseGemReward = 3;
        public int bonusGemPerStar = 1;
        public int bonusGemPerDifficulty = 1;

        [Header("Test thông số mặc định nếu không truyền vào")]
        public int defaultStarCount = 3;
        public int defaultDifficultyLevel = 0;

        [Header("X2 Collect")]
        public int x2Multiplier = 2;

        private int _currentGoldReward;
        private int _currentGemReward;
        private bool _isShowing = false;

        public void ShowReward()
        {
            ShowReward(defaultStarCount, defaultDifficultyLevel);
        }

        public void ShowReward(int starCount, int difficultyLevel)
        {
            starCount = Mathf.Clamp(starCount, 0, 3);
            difficultyLevel = Mathf.Max(0, difficultyLevel);

            _currentGoldReward = CalculateGoldReward(starCount, difficultyLevel);
            _currentGemReward = CalculateGemReward(starCount, difficultyLevel);

            if (goldAmountText != null)
                goldAmountText.text = _currentGoldReward.ToString();

            if (gemAmountText != null)
                gemAmountText.text = _currentGemReward.ToString();

            UpdateStars(starCount);

            if (panelRoot != null)
                panelRoot.SetActive(true);

            _isShowing = true;
            Time.timeScale = 0f;
        }

        private void UpdateStars(int starCount)
        {
            if (stars == null || stars.Length == 0) return;

            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                    stars[i].SetActive(i < starCount);
            }
        }

        private int CalculateGoldReward(int starCount, int difficultyLevel)
        {
            int gold = baseGoldReward;
            gold += starCount * bonusGoldPerStar;
            gold += difficultyLevel * bonusGoldPerDifficulty;
            return Mathf.Max(0, gold);
        }

        private int CalculateGemReward(int starCount, int difficultyLevel)
        {
            int gem = baseGemReward;
            gem += starCount * bonusGemPerStar;
            gem += difficultyLevel * bonusGemPerDifficulty;
            return Mathf.Max(0, gem);
        }

        /// <summary>
        /// Claim thưởng x1 và chuyển qua map còn lại:
        /// Map1 -> Map2, Map2 -> Map1.
        /// </summary>
        public void OnClickClaim()
        {
            if (!_isShowing) return;

            // Nhận thưởng x1
            GiveRewards(1);

            // Đóng panel + resume time
            ClosePanel();

            if (PlayerExperienceManager.Instance != null)
            {
                PlayerExperienceManager.Instance.currentLevel = 1;
            }

            // Đổi scene
            string current = SceneManager.GetActiveScene().name;
            string nextScene;

            if (current == "MainScene_Map1")
                nextScene = "MainScene_Map2";
            else if (current == "MainScene_Map2")
                nextScene = "MainScene_Map1";
            else
                nextScene = "MainScene_Map1"; // fallback

            SceneManager.LoadScene(nextScene);
        }

        /// <summary>
        /// Claim thưởng x2 và trở về Main Menu.
        /// Đồng thời reset Gold & Score trong run.
        /// </summary>
        public void OnClickX2Collect()
        {
            if (!_isShowing) return;

            // Nhận thưởng x2
            GiveRewards(x2Multiplier);

            // Reset Gold & Score trong run khi về Main Menu
            if (WalletManager.Instance != null)
            {
                WalletManager.Instance.ResetGoldForNewRun();
            }

            if (RunScoreManager.Instance != null)
            {
                RunScoreManager.Instance.ResetScoreForNewRun();
            }

            // Đóng panel + resume time
            ClosePanel();

            // Load Main Menu (đổi tên scene nếu anh đặt khác)
            SceneManager.LoadScene("MainMenuScene");
        }

        private void GiveRewards(int multiplier)
        {
            if (WalletManager.Instance == null)
            {
                Debug.LogWarning("[StageClearRewardUI] WalletManager.Instance == null, không thể cộng thưởng.");
                return;
            }

            int goldToAdd = _currentGoldReward * multiplier;
            int gemToAdd = _currentGemReward * multiplier;

            if (goldToAdd > 0)
            {
                WalletManager.Instance.AddCurrency(
                    CurrencyType.Gold,
                    goldToAdd,
                    multiplier == 1 ? "Stage Clear Gold" : "Stage Clear Gold X2"
                );
            }

            if (gemToAdd > 0)
            {
                WalletManager.Instance.AddCurrency(
                    CurrencyType.Gem,
                    gemToAdd,
                    multiplier == 1 ? "Stage Clear Gem" : "Stage Clear Gem X2"
                );
            }
        }

        private void ClosePanel()
        {
            _isShowing = false;

            if (panelRoot != null)
                panelRoot.SetActive(false);

            Time.timeScale = 1f;
        }
    }
}
