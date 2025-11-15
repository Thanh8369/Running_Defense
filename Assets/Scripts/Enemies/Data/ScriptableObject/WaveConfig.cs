using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Enemies/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public EnemyStats enemyStats;
        public int count;
        public float spawnDelay = 0.5f; // Delay giữa mỗi enemy trong group này
    }
    
    [Header("Wave Settings")]
    public int waveNumber;
    public float timeBetweenSpawns = 1f;
    public float prepareTime = 5f; // Thời gian chuẩn bị trước wave
    
    [Header("Enemy Groups")]
    public List<EnemySpawnInfo> enemyGroups = new List<EnemySpawnInfo>();
    
    [Header("Difficulty Scaling")]
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float goldMultiplier = 1f;
}
