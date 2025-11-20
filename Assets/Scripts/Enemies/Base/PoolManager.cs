using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    public List<PoolItem> poolItems = new List<PoolItem>();

    private Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        foreach (var item in poolItems)
        {
            if (item.prefab == null) continue;
            CreatePool(item.prefab, item.size);
        }
    }

    private void CreatePool(GameObject prefab, int size)
    {
        if (pools.ContainsKey(prefab)) return;

        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        pools[prefab] = queue;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(prefab))
        {
            CreatePool(prefab, 1);
        }

        Queue<GameObject> queue = pools[prefab];
        GameObject obj;

        if (queue.Count > 0)
            obj = queue.Dequeue();
        else
            obj = Instantiate(prefab, transform);

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        return obj;
    }

    public void Return(GameObject obj, GameObject prefab = null)
    {
        obj.SetActive(false);

        if (prefab != null && pools.ContainsKey(prefab))
        {
            pools[prefab].Enqueue(obj);
        }
        else
        {
            foreach (var kvp in pools)
            {
                if (kvp.Key.name == obj.name.Replace("(Clone)", "").Trim())
                {
                    kvp.Value.Enqueue(obj);
                    return;
                }
            }

            Destroy(obj);
        }
    }

    // thmap onlys
    [ContextMenu("Convert Pools")]
    public void ConvertPoolsToPoolItems()
    {
        foreach (var kvp in pools)
        {
            GameObject prefab = kvp.Key;

            if (poolItems.Exists(p => p.prefab == prefab)) continue;

            poolItems.Add(new PoolItem
            {
                prefab = prefab,
                size = 30
            });
        }
    }
}

[System.Serializable]
public class PoolItem
{
    public GameObject prefab;
    public int size = 10;
}
