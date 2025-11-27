using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerArea : MonoBehaviour
{
    public TowerRunStats runStats;
    [HideInInspector]
    public SphereCollider col;

    public List<Transform> enemyQueue = new List<Transform>();

   void Awake()
    {
        col = GetComponent<SphereCollider>();
        col.isTrigger = true;

        if (runStats != null)
            col.radius = runStats.attackRange; // <-- cập nhật từ runtime stats
    }

    public void UpdateRange()
    {
        if (runStats != null)
            col.radius = runStats.attackRange;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            enemyQueue.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            enemyQueue.Remove(other.transform);
    }
}