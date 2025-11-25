using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SupplySpawner : MonoBehaviour
{
    public static SupplySpawner Instance { get; private set; }

    [SerializeField] private float mapTimer = 0f;
    [SerializeField] private float spawnRadius = 3f;

    [Header("Spawn Config theo thời gian")]
    [SerializeField] private List<SupplySpawnConfig> spawnConfigs;
    [SerializeField] private Transform[] spawnPoints;

    public GameObject supplyPopupPrefab;
    public Canvas popupCanvas;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (spawnConfigs != null)
        {
            spawnConfigs.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        }
        StartCoroutine(RunTimeline());
    }

    private IEnumerator RunTimeline()
    {
        mapTimer = 0f;
        int index = 0;

        if (spawnConfigs == null)
        {
            Debug.LogWarning("[SupplySpawner] Chưa gán SpawnConfigs");
            yield break;
        }

        while (index < spawnConfigs.Count)
        {
            mapTimer += Time.deltaTime;

            while (index < spawnConfigs.Count &&
                   mapTimer >= spawnConfigs[index].triggerTime)
            {
                StartCoroutine(SpawnGroup(spawnConfigs[index]));
                index++;
            }

            yield return null;
        }

        Debug.LogWarning("[SupplySpawner] Tất cả supply đã spawn xong");
    }

    private IEnumerator SpawnGroup(SupplySpawnConfig config)
    {
        foreach (var info in config.spawnInfos)
        {
            StartCoroutine(SpawnSupplyGroup(info));
        }
        yield return null;
    }

    private IEnumerator SpawnSupplyGroup(SupplyInfo info)
    {
        int spawned = 0;

        while (spawned < info.count)
        {
            if (info.spawnDelay > 0)
                yield return new WaitForSeconds(info.spawnDelay);
            else
                yield return null;

            int batch = Random.Range(info.minBatch, info.maxBatch + 1);
            batch = Mathf.Min(batch, info.count - spawned);

            for (int i = 0; i < batch; i++)
            {
                SpawnSupply(info);
                spawned++;
            }
        }
    }

    private void SpawnSupply(SupplyInfo info)
    {
        if (spawnPoints.Length == 0) return;
        if (info.supply == null || info.supply.supplyPrefab == null) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = sp.position + new Vector3(offset.x, info.spawnHeightOffset, offset.y);

        GameObject supplyObj = PoolManager.Instance.Get(info.supply.supplyPrefab, spawnPos, Quaternion.identity);
        SupplyItem supplyItem = supplyObj.GetComponent<SupplyItem>();
        if (supplyItem != null)
        {
            supplyItem.Init(info.supply, spawnPos);
        }
    }
}
