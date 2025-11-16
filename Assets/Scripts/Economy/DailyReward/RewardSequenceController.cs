using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Điều khiển thứ tự bật các panel reward.
/// Gắn vào 1 GameObject trong scene (vd: RewardSequenceController).
/// </summary>
public class RewardSequenceController : MonoBehaviour
{
    [Header("Các panel reward theo thứ tự muốn hiển thị")]
    public StreakRewardUI weeklyReward;
    public StreakRewardUI monthlyReward;

    private Queue<StreakRewardUI> _queue = new Queue<StreakRewardUI>();
    private StreakRewardUI _current;

    private void Start()
    {
        // Ẩn hết ban đầu
        if (weeklyReward != null) weeklyReward.Hide();
        if (monthlyReward != null) monthlyReward.Hide();

        // Nếu hôm nay có thể claim Weekly → thêm vào queue
        if (weeklyReward != null && weeklyReward.CanClaimToday())
            _queue.Enqueue(weeklyReward);

        // Nếu hôm nay có thể claim Monthly → thêm vào queue
        if (monthlyReward != null && monthlyReward.CanClaimToday())
            _queue.Enqueue(monthlyReward);

        ShowNext();
    }

    /// <summary>
    /// Gọi sau khi 1 panel đã claim xong (gán trong OnClick của nút Claim).
    /// </summary>
    public void OnPanelClaimFinished()
    {
        if (_current != null)
        {
            _current.Hide();
            _current = null;
        }

        ShowNext();
    }

    private void ShowNext()
    {
        if (_queue.Count == 0) return;

        _current = _queue.Dequeue();
        _current.Show();
    }
}
