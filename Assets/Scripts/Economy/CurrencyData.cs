using UnityEngine;

[CreateAssetMenu(
    fileName = "Currency_",
    menuName = "RunningDefense/Economy/Currency Data",
    order = 1)]
public class CurrencyData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    [Tooltip("ID nội bộ, viết liền không dấu, dùng cho code / save. VD: gold, gem")]
    public string id;

    [Tooltip("Tên hiển thị cho người chơi. VD: Gold, Gem")]
    public string displayName;

    [TextArea]
    [Tooltip("Mô tả (dùng cho UI tooltip, nếu cần).")]
    public string description;

    [Header("Phân loại")]
    [Tooltip("Loại tiền tệ (Gold, Gem, v.v.)")]
    public CurrencyType currencyType;

    [Header("Hiển thị")]
    [Tooltip("Icon dùng trong UI (HUD, shop, popup...)")]
    public Sprite icon;

    /// <summary>
    /// Helper: dùng để debug cho dễ nhìn.
    /// </summary>
    public override string ToString()
    {
        return $"{displayName} ({currencyType})";
    }
}
