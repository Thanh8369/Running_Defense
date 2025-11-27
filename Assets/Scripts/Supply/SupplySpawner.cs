using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SupplySpawner : MonoBehaviour
{
    public static SupplySpawner Instance { get; private set; }

    [SerializeField] private float mapTimer = 0f;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private int totalActiveOnMap = 0;

    [Header("Spawn Config theo thời gian")]
    [SerializeField] private List<SupplySpawnConfig> spawnGroups;

    public GameObject supplyPopupPrefab;
    public Canvas popupCanvas;

    private Dictionary<SupplyData, int> activeCounts = new Dictionary<SupplyData, int>();
    private Dictionary<SupplySpawnConfig, int> groupActiveCounts = new Dictionary<SupplySpawnConfig, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ResetAllSupplyCounters();

        if (spawnGroups != null)
            spawnGroups.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));

        StartCoroutine(RunTimeline());
    }

    private void ResetAllSupplyCounters()
    {
        activeCounts.Clear();
        groupActiveCounts.Clear();
        totalActiveOnMap = 0;

        foreach (var group in spawnGroups)
        {
            if (!groupActiveCounts.ContainsKey(group))
                groupActiveCounts[group] = 0;

            if (group.supplyInfos == null) continue;
            foreach (var info in group.supplyInfos)
            {
                activeCounts[info.supply] = 0;
            }
        }
    }

    private IEnumerator RunTimeline()
    {
        mapTimer = 0f;
        int index = 0;

        while (index < spawnGroups.Count)
        {
            mapTimer += Time.deltaTime;

            while (index < spawnGroups.Count &&
                   mapTimer >= spawnGroups[index].triggerTime)
            {
                StartCoroutine(SpawnGroup(spawnGroups[index]));
                index++;
            }

            yield return null;
        }
    }

    private IEnumerator SpawnGroup(SupplySpawnConfig config)
    {
        int spawned = 0;

        while (spawned < config.totalSupplyCount)
        {
            if (groupActiveCounts[config] >= config.maxActiveSuppliesInMap)
            {
                yield return null;
                continue;
            }

            if (config.spawnDelay > 0)
                yield return new WaitForSeconds(config.spawnDelay);
            else
                yield return null;

            SupplyData supply = PickRandomSupply(config.supplyInfos);

            if (supply == null)
                continue;

            SpawnSupply(supply, config);
            spawned++;
        }
    }

    private SupplyData PickRandomSupply(SupplyInfo[] supplyInfos)
    {
        List<SupplyInfo> available = new List<SupplyInfo>();

        foreach (var info in supplyInfos)
        {
            int count = activeCounts.ContainsKey(info.supply) ? activeCounts[info.supply] : 0;
            if (count < info.maxActiveSupplies)
                available.Add(info);
        }

        if (available.Count == 0)
            return null;

        float totalChance = 0f;
        foreach (var a in available)
            totalChance += a.spawnChance;

        float rand = Random.value * totalChance;
        float sum = 0f;

        foreach (var a in available)
        {
            sum += a.spawnChance;
            if (rand < sum)
                return a.supply;
        }

        return available[available.Count - 1].supply;
    }

    private void SpawnSupply(SupplyData supply, SupplySpawnConfig config)
    {
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, 1.5f, offset.y);

        GameObject supplyObj = PoolManager.Instance.Get(supply.supplyPrefab, spawnPos, Quaternion.identity);

        SupplyItem item = supplyObj.GetComponent<SupplyItem>();
        if (item != null)
            item.Init(supply, spawnPos, this, config);

        // Per-type
        if (!activeCounts.ContainsKey(supply))
            activeCounts[supply] = 0;
        activeCounts[supply]++;

        // Per group
        groupActiveCounts[config]++;

        // Total
        totalActiveOnMap++;
    }

    public void OnSupplyPicked(SupplyData supply, SupplySpawnConfig config)
    {
        if (supply == null) return;

        if (activeCounts.ContainsKey(supply))
            activeCounts[supply] = Mathf.Max(0, activeCounts[supply] - 1);

        if (groupActiveCounts.ContainsKey(config))
            groupActiveCounts[config] = Mathf.Max(0, groupActiveCounts[config] - 1);

        totalActiveOnMap = Mathf.Max(0, totalActiveOnMap - 1);
    }
}
