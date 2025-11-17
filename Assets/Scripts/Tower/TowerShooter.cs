using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    public TowerArea tower;                 // phạm vi trụ
    public Transform firePoint;             // chỗ bắn đạn
    public GameObject bulletPrefab;         // prefab viên đạn
    public float attackCooldown = 1f;       // tốc độ bắn
    public float bulletSpeed = 12f;         // tốc độ đạn

    private float nextFireTime;
    private Transform target;

    void Update()
    {
        UpdateTarget();

        if (target == null) return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + attackCooldown;
        }
    }

    void UpdateTarget()
    {
        // xóa enemy null
        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        // chọn enemy đầu tiên (vừa bước vào)
        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || target == null)
            return;

        // tính hướng bắn
        Vector3 dir = (target.position - firePoint.position).normalized;

        // tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));

        // tạo chuyển động
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * bulletSpeed;
    }
}