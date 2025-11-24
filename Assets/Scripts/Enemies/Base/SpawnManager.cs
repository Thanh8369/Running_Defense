using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private float mapTimer = 0f;
    [SerializeField] private float spawnRadius = 5f;

    [Header("List Spawn Config theo thứ tự thời gian")]
    [SerializeField] private List<SpawnConfig> spawnConfigs;
    [SerializeField] private Transform[] spawnPoints;

    private HashSet<SpawnConfig> configsStarGranted = new(); // Đã cộng star chưa
    private int totalStars = 0;

    private int aliveEnemies = 0;
    private bool allSpawnsCompleted = false;

    public int TotalStars => totalStars;

    void Awake() => Instance = this;

    void Start()
    {
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
        Debug.LogWarning("All spawns completed.");
    }

    IEnumerator SpawnGroup(SpawnConfig config)
    {
        if (config.addStar && !configsStarGranted.Contains(config))
        {
            totalStars += 1;
            configsStarGranted.Add(config);
            Debug.LogWarning($"⭐ Config {config.name} spawned → +1 star. TotalStars = {totalStars}");
        }

        foreach (var info in config.spawnInfos)
        {
            StartCoroutine(SpawnEnemyGroup(info));
        }

        yield return null;
    }

    IEnumerator SpawnEnemyGroup(SpawnInfo info)
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
                SpawnEnemy(info);
                spawned++;
            }
        }
    }

    void SpawnEnemy(SpawnInfo info)
    {
        if (spawnPoints.Length == 0) return;

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = sp.position + new Vector3(offset.x, info.spawnHeightOffset, offset.y);

        GameObject enemyObj = PoolManager.Instance.Get(info.enemy.prefab, spawnPos, Quaternion.identity);

        EnemyHealth hp = enemyObj.GetComponent<EnemyHealth>();
        hp.onDie += () => { aliveEnemies--; };

        aliveEnemies++;
    }

    public void AddStar()
    {
        totalStars += 1;
        Debug.LogWarning($"⭐ AddStar() → TotalStars = {totalStars}");
    }
}
