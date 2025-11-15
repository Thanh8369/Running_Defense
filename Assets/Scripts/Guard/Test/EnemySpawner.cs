using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform tower;
    public GameObject enemyPrefab;
    public float spawnDistance = 20f;

    private float timer;
    public float spawnInterval = 3f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int dir = Random.Range(0, 5);
        Vector3 offset = Vector3.zero;

        switch (dir)
        {
            case 0: offset = Vector3.left * spawnDistance; break;
            case 1: offset = Vector3.right * spawnDistance; break;
            case 2: offset = Vector3.forward * spawnDistance; break;
            case 3: offset = Vector3.back * spawnDistance; break;
        }

        Vector3 pos = tower.position + offset;

        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        // 🟢 Gán target để enemy tự chạy vào trụ
        EnemyMove em = enemy.GetComponent<EnemyMove>();
        if (em != null)
            em.SetTarget(tower);
    }
}