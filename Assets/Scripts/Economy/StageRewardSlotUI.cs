using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageRewardSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI amountText;

    private RewardItem rewardData;

    public void Setup(RewardItem data)
    {
        rewardData = data;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (amountText != null)
            amountText.text = data.amount.ToString();
    }

    public RewardItem GetReward()
    {
        return rewardData;
    }
}
