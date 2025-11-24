using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnConfig", menuName = "Enemy/Spawn Config")]
public class SpawnConfig : ScriptableObject
{
    public float triggerTime = 0f;
    public bool addStar = false;
    public List<SpawnInfo> spawnInfos = new List<SpawnInfo>();
}

[System.Serializable]
public class SpawnInfo
{
    public EnemyStats enemy;
    public int count = 1;
    public int minBatch = 1;
    public int maxBatch = 5;

    public float spawnDelay = 0.5f;
    public float spawnHeightOffset = 0f;
}
