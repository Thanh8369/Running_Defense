using System;
using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Quản lý điểm số trong 1 run + lưu best score.
    /// - CurrentScore: điểm hiện tại của run (reset mỗi lần chơi lại).
    /// - BestScore   : điểm cao nhất từng đạt được (lưu bằng PlayerPrefs).
    /// 
    /// Dùng cho:
    /// - Cộng điểm khi tiêu diệt quái.
    /// - Hiển thị điểm cuối run.
    /// - So sánh & cập nhật kỷ lục.
    /// </summary>
    public class RunScoreManager : MonoBehaviour
    {
        public static RunScoreManager Instance { get; private set; }

        [Header("Runtime Score")]
        [Tooltip("Điểm hiện tại trong run (reset mỗi lần bắt đầu run mới).")]
        [SerializeField] private int currentScore = 0;
        public int CurrentScore => currentScore;

        [Header("Best Score")]
        [Tooltip("Điểm cao nhất từng đạt được (load từ PlayerPrefs).")]
        [SerializeField] private int bestScore = 0;
        public int BestScore => bestScore;

        /// <summary>
        /// Event gọi khi currentScore thay đổi.
        /// UI có thể subscribe để update text.
        /// </summary>
        public event Action<int> OnScoreChanged;

        /// <summary>
        /// Event gọi khi bestScore được cập nhật.
        /// UI có thể hiển thị hiệu ứng "New Record".
        /// </summary>
        public event Action<int> OnBestScoreUpdated;

        private const string BEST_SCORE_KEY = "RD_BestScore_v1";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadBestScore();
            ResetScoreForNewRun(); // đảm bảo start run với 0 điểm
        }

        /// <summary>
        /// Reset điểm hiện tại về 0 (gọi khi bắt đầu 1 run mới).
        /// </summary>
        public void ResetScoreForNewRun()
        {
            currentScore = 0;
            OnScoreChanged?.Invoke(currentScore);
            Debug.Log("[RunScoreManager] ResetScoreForNewRun → Score = 0");
        }

        /// <summary>
        /// Cộng điểm (ví dụ khi tiêu diệt quái).
        /// amount phải là số dương.
        /// </summary>
        public void AddScore(int amount, string reason = "")
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[RunScoreManager] AddScore nhận amount <= 0 ({amount}), bỏ qua.");
                return;
            }

            int oldScore = currentScore;
            long newScoreLong = (long)oldScore + amount;

            // Clamp tránh overflow
            if (newScoreLong > int.MaxValue)
            {
                newScoreLong = int.MaxValue;
                Debug.LogWarning("[RunScoreManager] Score vượt quá int.MaxValue, đã clamp.");
            }

            currentScore = (int)newScoreLong;

            string reasonText = string.IsNullOrEmpty(reason) ? "" : $" | Reason: {reason}";
            Debug.Log($"[RunScoreManager] ADD +{amount} SCORE | Old={oldScore} → New={currentScore}{reasonText}");

            OnScoreChanged?.Invoke(currentScore);

            // Sau khi update currentScore, kiểm tra có phá kỷ lục không
            TryUpdateBestScore();
        }

        /// <summary>
        /// Kiểm tra và cập nhật BestScore nếu CurrentScore lớn hơn.
        /// </summary>
        private void TryUpdateBestScore()
        {
            if (currentScore <= bestScore) return;

            bestScore = currentScore;
            SaveBestScore();

            Debug.Log($"[RunScoreManager] New BEST SCORE = {bestScore}");
            OnBestScoreUpdated?.Invoke(bestScore);
        }

        #region SAVE / LOAD BEST SCORE

        private void SaveBestScore()
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, bestScore);
            PlayerPrefs.Save();
            Debug.Log($"[RunScoreManager] SaveBestScore → {bestScore}");
        }

        private void LoadBestScore()
        {
            if (!PlayerPrefs.HasKey(BEST_SCORE_KEY))
            {
                bestScore = 0;
                Debug.Log("[RunScoreManager] Chưa có BestScore, mặc định = 0.");
                return;
            }

            bestScore = Mathf.Max(0, PlayerPrefs.GetInt(BEST_SCORE_KEY, 0));
            Debug.Log($"[RunScoreManager] LoadBestScore → {bestScore}");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveBestScore();
            }
        }

        private void OnApplicationQuit()
        {
            SaveBestScore();
        }

        #endregion
    }
}
