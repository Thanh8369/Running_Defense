using UnityEngine;

public enum RewardType
{
    Gold,
    Gem,
    Item   // để dành cho tương lai
}

[System.Serializable]
public class RewardItem
{
    public RewardType type;
    public int amount;
    public Sprite icon;
}
