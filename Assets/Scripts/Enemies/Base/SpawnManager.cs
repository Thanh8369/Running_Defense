using Son.Economy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    public StageClearRewardUI UIReward;

    [SerializeField] private float mapTimer = 0f;
    [SerializeField] private float spawnRadius = 5f;

    [Header("List Spawn Config theo thời gian")]
    [SerializeField] private List<SpawnConfig> spawnConfigs;

    [SerializeField] private Transform[] spawnPoints;

    private int aliveEnemies = 0;
    private bool allSpawnsCompleted = false;

    void Awake()
    {
        Instance = this;
        UIReward = FindAnyObjectByType<StageClearRewardUI>();
    }

    void Start()
    {
        // Sort theo trigger time
        spawnConfigs.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        StartCoroutine(RunTimeline());
    }

    IEnumerator RunTimeline()
    {
        mapTimer = 0f;
        int index = 0;

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

        allSpawnsCompleted = true;
        Debug.Log("All spawn groups have been triggered.");
    }

    IEnumerator SpawnGroup(SpawnConfig config)
    {
        List<Coroutine> runningGroups = new List<Coroutine>();

        foreach (var info in config.spawnInfos)
        {
            Coroutine c = StartCoroutine(SpawnEnemyGroup(info));
            runningGroups.Add(c);
        }

        foreach (var group in runningGroups)
            yield return group;
    }

    IEnumerator SpawnEnemyGroup(SpawnInfo info)
    {
        int spawned = 0;

        while (spawned < info.count)
        {
            int batchSize = Random.Range(info.minBatch, info.maxBatch + 1);
            batchSize = Mathf.Min(batchSize, info.count - spawned);

            for (int i = 0; i < batchSize; i++)
            {
                SpawnEnemy(info);
                spawned++;
            }

            // Delay giữa các batch
            if (info.spawnDelay > 0)
                yield return new WaitForSeconds(info.spawnDelay);
            else
                yield return null; // ít nhất 1 frame để không block
        }
    }

    void SpawnEnemy(SpawnInfo info)
    {
        if (spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = sp.position + new Vector3(offset.x, info.spawnHeightOffset, offset.y);

        PoolManager.Instance.Get(info.enemy.prefab, spawnPos, Quaternion.identity);
        aliveEnemies++;
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;

        if (allSpawnsCompleted && aliveEnemies <= 0)
        {
            //UIReward.ShowReward();
            Debug.Log("GAME COMPLETED!!!");
        }
    }
}
