using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Quản lý UI STAGE CLEAR + thưởng Gold & Gem.
/// - Hiển thị số Gold & Gem nhận được.
/// - Nút Claim: nhận thưởng 1x.
/// - Nút X2 Collect: nhận thưởng x2 (sau này có thể gắn Ads).
/// </summary>
public class StageClearRewardUI : MonoBehaviour
{
    [Header("Root panel của Stage Clear")]
    [Tooltip("GameObject gốc chứa toàn bộ UI Stage Clear.")]
    public GameObject panelRoot;

    [Header("UI hiển thị Gold")]
    [Tooltip("Text hiển thị số Gold thưởng.")]
    public TextMeshProUGUI goldAmountText;

    [Tooltip("Icon Gold (coin).")]
    public Image goldIconImage;

    [Header("UI hiển thị Gem")]
    [Tooltip("Text hiển thị số Gem thưởng.")]
    public TextMeshProUGUI gemAmountText;

    [Tooltip("Icon Gem.")]
    public Image gemIconImage;

    [Header("Cấu hình thưởng Gold cơ bản")]
    [Tooltip("Gold thưởng cơ bản mỗi lần clear stage.")]
    public int baseGoldReward = 1500;

    [Tooltip("Gold thêm cho mỗi sao.")]
    public int bonusGoldPerStar = 100;

    [Tooltip("Gold thêm cho mỗi cấp độ khó.")]
    public int bonusGoldPerDifficulty = 200;

    [Header("Cấu hình thưởng Gem cơ bản")]
    [Tooltip("Gem thưởng cơ bản mỗi lần clear stage.")]
    public int baseGemReward = 3;

    [Tooltip("Gem thêm cho mỗi sao.")]
    public int bonusGemPerStar = 1;

    [Tooltip("Gem thêm cho mỗi cấp độ khó.")]
    public int bonusGemPerDifficulty = 1;

    [Header("Test thông số mặc định nếu không truyền vào")]
    [Tooltip("Số sao mặc định dùng khi gọi ShowReward() không có tham số.")]
    public int defaultStarCount = 3;

    [Tooltip("Độ khó mặc định nếu không truyền vào.")]
    public int defaultDifficultyLevel = 0;

    [Header("X2 Collect")]
    [Tooltip("Hệ số nhân khi X2 Collect.")]
    public int x2Multiplier = 2;

    private int _currentGoldReward;
    private int _currentGemReward;
    private bool _isShowing = false;

    /// <summary>
    /// Gọi hàm này khi STAGE CLEAR.
    /// Nếu không truyền star/difficulty sẽ dùng defaultStarCount + defaultDifficultyLevel.
    /// </summary>
    public void ShowReward()
    {
        ShowReward(defaultStarCount, defaultDifficultyLevel);
    }

    /// <summary>
    /// Gọi hàm này khi STAGE CLEAR, có truyền số sao & độ khó.
    /// </summary>
    public void ShowReward(int starCount, int difficultyLevel)
    {
        starCount = Mathf.Clamp(starCount, 0, 3);
        difficultyLevel = Mathf.Max(0, difficultyLevel);

        _currentGoldReward = CalculateGoldReward(starCount, difficultyLevel);
        _currentGemReward = CalculateGemReward(starCount, difficultyLevel);

        if (goldAmountText != null)
        {
            goldAmountText.text = _currentGoldReward.ToString();
        }

        if (gemAmountText != null)
        {
            gemAmountText.text = _currentGemReward.ToString();
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        _isShowing = true;

        // Tùy bạn: có thể pause game khi hiện UI thắng
        Time.timeScale = 0f;

        Debug.Log($"[StageClearRewardUI] ShowReward → Gold = {_currentGoldReward}, Gem = {_currentGemReward} (stars={starCount}, diff={difficultyLevel})");
    }

    /// <summary>
    /// Công thức tính Gold thưởng.
    /// </summary>
    private int CalculateGoldReward(int starCount, int difficultyLevel)
    {
        int gold = baseGoldReward;
        gold += starCount * bonusGoldPerStar;
        gold += difficultyLevel * bonusGoldPerDifficulty;
        return Mathf.Max(0, gold);
    }

    /// <summary>
    /// Công thức tính Gem thưởng.
    /// </summary>
    private int CalculateGemReward(int starCount, int difficultyLevel)
    {
        int gem = baseGemReward;
        gem += starCount * bonusGemPerStar;
        gem += difficultyLevel * bonusGemPerDifficulty;
        return Mathf.Max(0, gem);
    }

    /// <summary>
    /// Button "Claim" → nhận Gold + Gem 1x.
    /// Gán hàm này vào OnClick của nút Claim trong UI.
    /// </summary>
    public void OnClickClaim()
    {
        if (!_isShowing) return;

        GiveRewards(1);
        ClosePanel();
    }

    /// <summary>
    /// Button "X2 Collect" → nhận Gold + Gem x2.
    /// Hiện tại cho free; sau này bạn có thể gắn Ads tại đây.
    /// </summary>
    public void OnClickX2Collect()
    {
        if (!_isShowing) return;

        GiveRewards(x2Multiplier);
        ClosePanel();
    }

    /// <summary>
    /// Cộng thưởng vào WalletManager theo multiplier (1x hoặc 2x).
    /// </summary>
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

        Debug.Log($"[StageClearRewardUI] Claim rewards → Gold={goldToAdd}, Gem={gemToAdd}");
    }

    /// <summary>
    /// Đóng panel, resume game.
    /// </summary>
    private void ClosePanel()
    {
        _isShowing = false;

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        // Resume game
        Time.timeScale = 1f;
    }
}
