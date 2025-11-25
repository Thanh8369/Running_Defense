using UnityEngine;

public enum SupplyType
{
    Gold,
    Experience,
    PlayerHeal,
    TowerHeal,
    TowerDamage
}

[CreateAssetMenu(fileName = "SupplyData", menuName = "Supply/Supply Data")]
public class SupplyData : ScriptableObject
{
    public SupplyType supplyType;
    public int defaultAmount;

    [Header("Random")]
    public bool useRandomAmount = false;
    public int minAmount;
    public int maxAmount;

    [Header("Popup")]
    public bool showPopupOnPickup = true;
    public float popupDuration = 1f;
    public float popupMoveDistance = 50f;
    public Sprite icon;
    public string textDisplayName;
    public int textFontSize;
    public Color textColor;

    [Header("Prefab")]
    public GameObject supplyPrefab;

    public int GetFinalAmount()
    {
        if (useRandomAmount)
        {
            return Random.Range(minAmount, maxAmount + 1);
        }
        return defaultAmount;
    }
}
