using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private float mapDuration = 480f;
    [SerializeField] private float mapTimer = 0f;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private SpawnConfig spawnConfig;
    [SerializeField] private Transform[] spawnPoints;

    private int aliveEnemies = 0;
    private bool allSpawnsCompleted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RunSpawns());
    }

    IEnumerator RunSpawns()
    {
        mapTimer = 0f;

        if (spawnConfig == null || spawnConfig.spawnInfos.Count == 0)
            yield break;

        spawnConfig.spawnInfos.Sort((a, b) => a.triggerTime.CompareTo(b.triggerTime));
        int spawnIndex = 0;

        while (spawnIndex < spawnConfig.spawnInfos.Count)
        {
            mapTimer += Time.deltaTime;

            while (spawnIndex < spawnConfig.spawnInfos.Count &&
                   mapTimer >= spawnConfig.spawnInfos[spawnIndex].triggerTime)
            {
                StartCoroutine(SpawnEnemyGroup(spawnConfig.spawnInfos[spawnIndex]));
                spawnIndex++;
            }

            yield return null;
        }

        allSpawnsCompleted = true;
        Debug.Log("All enemies from spawnConfig have spawned.");
    }

    IEnumerator SpawnEnemyGroup(SpawnInfo info)
    {
        for (int i = 0; i < info.count; i++)
        {
            SpawnEnemy(info);
            yield return new WaitForSeconds(info.spawnDelay);
        }
    }

    void SpawnEnemy(SpawnInfo info)
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
        float heightOffset = info.spawnHeightOffset;

        Vector3 spawnPos = spawnPoint.position + new Vector3(offset2D.x, heightOffset, offset2D.y);
        PoolManager.Instance.Get(info.enemy.prefab, spawnPos, Quaternion.identity);

        aliveEnemies++;
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;

        if (allSpawnsCompleted && aliveEnemies <= 0)
        {
            OnGameWin();
        }
    }

    void OnGameWin()
    {
        Debug.Log("GAME COMPLETED! All enemies defeated!");
        Time.timeScale = 0f;
    }
}
