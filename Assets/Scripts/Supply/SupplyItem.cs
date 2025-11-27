using UnityEngine;
using Son.Economy;

public class SupplyItem : MonoBehaviour
{
    public SupplyData data;
    private SupplySpawner spawner;
    private SupplySpawnConfig group;

    private int finalAmount;
    private Vector3 startPos;

    private void Update()
    {
        Vector3 newPos = startPos;
        newPos.y += Mathf.Sin(Time.time * 2f) * .3f;
        transform.position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ApplySupply();
        spawner?.OnSupplyPicked(data, group);

        PoolManager.Instance.Return(gameObject);
    }

    public void Init(SupplyData supplyData, Vector3 spawnPosition, SupplySpawner spawner, SupplySpawnConfig group)
    {
        data = supplyData;
        this.spawner = spawner;
        this.group = group;

        startPos = spawnPosition;
        transform.position = spawnPosition;

        finalAmount = data != null ? data.GetFinalAmount() : 0;
    }

    private void ApplySupply()
    {
        if (data == null) return;

        if (data.showPopupOnPickup)
            ShowPopup(finalAmount);

        switch (data.supplyType)
        {
            case SupplyType.Gold:
                WalletManager.Instance?.AddCurrency(CurrencyType.Gold, finalAmount);
                break;
            case SupplyType.Experience:
                PlayerExperienceManager.Instance?.AddExp(finalAmount);
                break;
            case SupplyType.PlayerHeal:
                GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>()?.Heal(finalAmount);
                break;
            case SupplyType.TowerHeal:
                GameObject.FindGameObjectWithTag("Tower")?.GetComponent<Health>()?.Heal(finalAmount);
                break;
            case SupplyType.TowerDamage:
                DamageTowerArea();
                break;
        }
    }

    private void DamageTowerArea()
    {
        TowerArea towerArea = GameObject.FindGameObjectWithTag("Tower")?.GetComponent<TowerArea>();
        if (towerArea == null) return;

        foreach (Transform enemy in towerArea.enemyQueue)
        {
            if (enemy == null) continue;
            enemy.GetComponent<IDamageable>()?.TakeDamage(finalAmount);
        }
    }

    private void ShowPopup(int amount)
    {
        if (SupplySpawner.Instance == null) return;

        Canvas canvas = SupplySpawner.Instance.popupCanvas;
        GameObject popupPrefab = SupplySpawner.Instance.supplyPopupPrefab;
        if (canvas == null || popupPrefab == null) return;

        GameObject popupObj = Instantiate(popupPrefab, canvas.transform);
        RectTransform rect = popupObj.GetComponent<RectTransform>();
        if (rect != null)
            rect.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);

        popupObj.GetComponent<SupplyPopup>()?.Init(amount, data);
    }
}
