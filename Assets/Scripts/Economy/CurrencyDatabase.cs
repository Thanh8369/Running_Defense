using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Database chứa tất cả các loại tiền tệ của game.
/// Dùng để tra cứu thông tin Currency theo loại (Gold, Gem).
/// </summary>
[CreateAssetMenu(
    fileName = "CurrencyDatabase",
    menuName = "RunningDefense/Economy/Currency Database",
    order = 2)]
public class CurrencyDatabase : ScriptableObject
{
    [Tooltip("Danh sách tất cả các loại tiền trong game (Gold, Gem, ...).")]
    public List<CurrencyData> currencies = new List<CurrencyData>();

    // Dictionary để tra cứu nhanh theo CurrencyType.
    private Dictionary<CurrencyType, CurrencyData> _byType;

    private void OnEnable()
    {
        BuildLookup();
    }

    /// <summary>
    /// Xây lại dictionary mỗi khi asset được load.
    /// </summary>
    private void BuildLookup()
    {
        _byType = new Dictionary<CurrencyType, CurrencyData>();

        foreach (var c in currencies)
        {
            if (c == null)
                continue;

            if (_byType.ContainsKey(c.currencyType))
            {
                Debug.LogWarning($"[CurrencyDatabase] Trùng loại tiền: {c.currencyType}. " +
                                 $"Currency cũ sẽ bị ghi đè bởi {c.name}.");
            }

            _byType[c.currencyType] = c;
        }
    }

    /// <summary>
    /// Lấy CurrencyData theo CurrencyType.
    /// </summary>
    public CurrencyData GetCurrency(CurrencyType type)
    {
        if (_byType == null || _byType.Count == 0)
        {
            BuildLookup();
        }

        if (_byType.TryGetValue(type, out var data))
        {
            return data;
        }

        Debug.LogError($"[CurrencyDatabase] Không tìm thấy CurrencyData cho loại: {type}");
        return null;
    }

    /// <summary>
    /// Lấy CurrencyData theo id text (vd: "gold", "gem").
    /// </summary>
    public CurrencyData GetCurrencyById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (var c in currencies)
        {
            if (c != null && c.id == id)
                return c;
        }

        Debug.LogError($"[CurrencyDatabase] Không tìm thấy CurrencyData với id: {id}");
        return null;
    }
}
