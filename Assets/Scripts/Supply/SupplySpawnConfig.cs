using System;
using UnityEngine;

[Serializable]
public class SupplyInfo
{
    public SupplyData supply;
    [Range(0f, 100f)] public float spawnChance = 50f;
    public int maxActiveSupplies;
}

[CreateAssetMenu(fileName = "SupplySpawnConfig", menuName = "Supply/Supply Spawn Config")]
public class SupplySpawnConfig : ScriptableObject
{
    public float triggerTime = 0f;

    public int totalSupplyCount;
    public int maxActiveSuppliesInMap;
    public float spawnDelay;

    public SupplyInfo[] supplyInfos;
}
