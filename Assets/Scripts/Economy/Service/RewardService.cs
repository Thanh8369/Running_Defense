// gợi ý class
using UnityEngine;

public class RewardService : MonoBehaviour
{
    public static RewardService Instance { get; private set; }
    public WalletManager wallet;

    private void Awake()
    {
        Instance = this;
    }

    public void RewardStageClear(int stageIndex, int stars)
    {
        int gold = 200 + 50 * stageIndex;
        int gem = stars >= 3 ? 5 : 0;

        wallet.AddCurrency(CurrencyType.Gold, gold, $"StageClear_{stageIndex}");
        if (gem > 0)
            wallet.AddCurrency(CurrencyType.Gem, gem, $"StageClear_{stageIndex}_3Stars");
    }

    public void RewardAdGem(int amount)
    {
        wallet.AddCurrency(CurrencyType.Gem, amount, "AdReward");
    }

    //public void RewardDailyLogin(DailyRewardConfig reward)
    //{
    //    wallet.AddCurrency(reward.currency, reward.amount, "DailyLogin");
    //}
}
