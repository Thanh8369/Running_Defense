using System.Collections.Generic;
using UnityEngine;

public class DaggerObjectPool : MonoBehaviour
{
    public static DaggerObjectPool Instance;

    [Header("Pool Settings")]
    public GameObject prefab;
    public int initialSize = 20;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            // Expand pool if needed
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }

        GameObject item = pool.Dequeue();
        item.SetActive(true);
        return item;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
