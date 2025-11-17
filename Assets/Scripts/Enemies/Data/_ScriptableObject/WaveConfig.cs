using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Enemy/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public float prepareTime = 5f;
    public List<EnemySpawnInfo> enemyGroups = new List<EnemySpawnInfo>();
}

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyStats enemyStats;
    public int count;
    public float spawnDelay = 0.5f;
    public float spawnHeightOffset = 0f;
}
