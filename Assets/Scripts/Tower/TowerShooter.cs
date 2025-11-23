using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    public TowerArea tower;
    public TowerData towerStats;

    public Transform firePoint;
    public GameObject bulletPrefab;

    public float attackCooldown = 1f;
    public float bulletSpeed = 12f;

    private float nextFireTime;
    private Transform target;

    private void Start()
    {
        if (tower == null)
            tower = GetComponent<TowerArea>();

        if (tower == null)
            Debug.LogError("TowerShooter: Không tìm thấy TowerArea!");
    }

    private void Update()
    {
        UpdateTarget();
        if (target == null) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + attackCooldown;
        }
    }

    private void UpdateTarget()
    {
        // LOẠI BỎ ENEMY CHẾT / DISABLE
        tower.enemyQueue.RemoveAll(e =>
            e == null ||
            !e.gameObject.activeInHierarchy ||
            e.GetComponent<EnemyHealth>()?.IsDead() == true
        );

        // KHÔNG CÓ ENEMY → RESET TARGET
        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        // TARGET NULL HOẶC KHÔNG CÒN TRONG QUEUE → CHỌN TARGET MỚI
        if (target == null || !tower.enemyQueue.Contains(target))
        {
            target = tower.enemyQueue[0];
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || target == null) return;

        TowerRunStats runStats = FindAnyObjectByType<TowerRunStats>();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletTower bt = bullet.GetComponent<BulletTower>();
        if (bt != null)
        {
            bt.Init(target, runStats);
        }
    }
}