using UnityEngine;

public class CurrencyDebugTester : MonoBehaviour
{
    [Header("Kéo CurrencyDatabase_Main vào đây")]
    public CurrencyDatabase currencyDatabase;

    private void Start()
    {
        if (currencyDatabase == null)
        {
            Debug.LogError("[CurrencyDebugTester] Chưa gán CurrencyDatabase!");
            return;
        }

        var gold = currencyDatabase.GetCurrency(CurrencyType.Gold);
        var gem = currencyDatabase.GetCurrency(CurrencyType.Gem);

        Debug.Log($"[CurrencyDebugTester] Gold: {gold}");
        Debug.Log($"[CurrencyDebugTester] Gem : {gem}");
    }
}
