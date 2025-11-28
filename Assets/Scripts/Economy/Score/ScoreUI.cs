using TMPro;
using UnityEngine;
using Son.Economy;

public class ScoreUI : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI bestScoreText;

    private void Start()
    {
        if (RunScoreManager.Instance != null)
        {
            // Update UI ngay lập tức theo giá trị hiện tại
            UpdateCurrentScore(RunScoreManager.Instance.CurrentScore);
            UpdateBestScore(RunScoreManager.Instance.BestScore);

            // Đăng ký lắng nghe event
            RunScoreManager.Instance.OnScoreChanged += UpdateCurrentScore;
            RunScoreManager.Instance.OnBestScoreUpdated += UpdateBestScore;
        }
        else
        {
            Debug.LogWarning("[ScoreUI] RunScoreManager.Instance = null");
        }
    }

    private void OnDestroy()
    {
        if (RunScoreManager.Instance != null)
        {
            RunScoreManager.Instance.OnScoreChanged -= UpdateCurrentScore;
            RunScoreManager.Instance.OnBestScoreUpdated -= UpdateBestScore;
        }
    }

    private void UpdateCurrentScore(int score)
    {
        if (currentScoreText != null)
            currentScoreText.text = score.ToString();
    }

    private void UpdateBestScore(int best)
    {
        if (bestScoreText != null)
            bestScoreText.text = best.ToString();
    }
}


//// ✅ Cộng điểm khi quái chết
//if (RunScoreManager.Instance != null)
//{
//    RunScoreManager.Instance.AddScore(scoreOnDeath, $"Kill {gameObject.name}");
//}