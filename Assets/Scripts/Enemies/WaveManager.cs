using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveConfig> waves = new List<WaveConfig>();
    public Transform[] spawnPoints;

    private float spawnRadius = 3f;
    private int currentWaveIndex = 0;

    void Start()
    {
        StartCoroutine(WaveSequence());
    }

    IEnumerator WaveSequence()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveConfig currentWave = waves[currentWaveIndex];

            // Prepare for wave
            yield return new WaitForSeconds(currentWave.prepareTime);

            // Spawn wave
            yield return StartCoroutine(SpawnWave(currentWave));

            // Move to next wave
            currentWaveIndex++;
        }

        Debug.Log("All waves completed! Victory!");
    }

    IEnumerator SpawnWave(WaveConfig wave)
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();

        foreach (var info in wave.enemyGroups)
        {
            Coroutine c = StartCoroutine(SpawnEnemyGroup(info));
            runningCoroutines.Add(c);
        }

        foreach (var c in runningCoroutines)
            yield return c;
    }

    IEnumerator SpawnEnemyGroup(EnemySpawnInfo info)
    {
        for (int i = 0; i < info.count; i++)
        {
            SpawnEnemy(info);
            yield return new WaitForSeconds(info.spawnDelay);
        }
    }

    void SpawnEnemy(EnemySpawnInfo info)
    {
        if (spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector2 offset2D = Random.insideUnitCircle * spawnRadius;

        float heightOffset = info.spawnHeightOffset;
        Vector3 spawnPos = spawnPoint.position +
                           new Vector3(offset2D.x, heightOffset, offset2D.y);

        GameObject enemyObj = Instantiate(info.enemyStats.prefab, spawnPos, Quaternion.identity);

        EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.Initialize(info.enemyStats);
        }
    }
}
