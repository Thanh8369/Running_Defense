using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    [SerializeField] private float spawnRadius = 3f;
    public List<WaveConfig> waves = new List<WaveConfig>();
    public Transform[] spawnPoints;

    [Header("Difficulty Scaling")]
    public float healthScalePerWave = 1.15f; // +15% mỗi wave
    public float damageScalePerWave = 1.1f; // +10% mỗi wave
    public float speedScalePerWave = 1.05f; // +5% mỗi wave

    [Header("Runtime")]
    public int currentWaveIndex = 0;
    public bool isWaveActive = false;
    public int enemiesRemaining = 0;

    void Start()
    {
        StartCoroutine(WaveSequence());
    }

    IEnumerator WaveSequence()
    {
        while (currentWaveIndex < waves.Count)
        {
            WaveConfig currentWave = waves[currentWaveIndex];

            // Prepare phase
            Debug.Log($"Wave {currentWaveIndex + 1} starting in {currentWave.prepareTime} seconds!");
            yield return new WaitForSeconds(currentWave.prepareTime);

            // Spawn wave
            isWaveActive = true;
            yield return StartCoroutine(SpawnWave(currentWave));

            // Wait for all enemies to be defeated
            while (enemiesRemaining > 0)
                yield return null;

            isWaveActive = false;
            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                Debug.Log($"Wave {currentWaveIndex} completed! Next wave incoming...");
                yield return new WaitForSeconds(3f);
            }
        }

        Debug.Log("All waves completed! Victory!");
    }

    IEnumerator SpawnWave(WaveConfig wave)
    {
        float currentHealthMult = wave.healthMultiplier * Mathf.Pow(healthScalePerWave, currentWaveIndex);
        float currentDamageMult = wave.damageMultiplier * Mathf.Pow(damageScalePerWave, currentWaveIndex);
        float currentSpeedMult = wave.speedMultiplier * Mathf.Pow(speedScalePerWave, currentWaveIndex);

        List<Coroutine> runningCoroutines = new List<Coroutine>();

        foreach (var group in wave.enemyGroups)
        {
            // Start một Coroutine riêng cho từng group
            Coroutine c = StartCoroutine(SpawnEnemyGroup(group, currentHealthMult, currentDamageMult, currentSpeedMult));
            runningCoroutines.Add(c);
        }

        // Chờ tất cả group spawn xong
        foreach (var c in runningCoroutines)
            yield return c;
    }

    IEnumerator SpawnEnemyGroup(WaveConfig.EnemySpawnInfo group, float healthMult, float damageMult, float speedMult)
    {
        for (int i = 0; i < group.count; i++)
        {
            SpawnEnemy(group.enemyStats, healthMult, damageMult, speedMult);
            yield return new WaitForSeconds(group.spawnDelay);
        }
    }


    void SpawnEnemy(EnemyStats stats, float healthMult, float damageMult, float speedMult)
{
    if (spawnPoints.Length == 0) return;

    Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

    // Random trong vòng tròn quanh spawnPoint
    Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
    Vector3 spawnPos = spawnPoint.position + new Vector3(offset2D.x, 0f, offset2D.y);

    GameObject enemyObj = Instantiate(stats.prefab, spawnPos, Quaternion.identity);

    EnemyAI enemy = enemyObj.GetComponent<EnemyAI>();
    if (enemy != null)
    {
        enemy.Initialize(stats, healthMult, damageMult, speedMult);
        enemiesRemaining++;
    }
}
}
