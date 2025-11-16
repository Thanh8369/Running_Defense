using System.Collections.Generic;
using UnityEngine;

public class TowerArea : MonoBehaviour
{
    public float range = 10f;
    public List<Transform> enemyQueue = new List<Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
            enemyQueue.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            enemyQueue.Remove(other.transform);
    }

    private void Update()
    {
        // Remove tất cả enemy đã bị Destroy (null) ra khỏi list
        enemyQueue.RemoveAll(e => e == null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}