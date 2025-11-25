using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SupplyInfo
{
    public SupplyData supply;
    public int count = 1;
    public int minBatch = 1;
    public int maxBatch = 1;
    public float spawnDelay = 0.5f;
    public float spawnHeightOffset = 0.5f;
}


[CreateAssetMenu(fileName = "SupplySpawnConfig", menuName = "Supply/Supply Spawn Config")]
public class SupplySpawnConfig : ScriptableObject
{
    public float triggerTime = 0f;
    public List<SupplyInfo> spawnInfos = new List<SupplyInfo>();
}