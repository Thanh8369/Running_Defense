using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    public TowerArea tower;             // phạm vi trụ
    public Transform firePoint;         // chỗ bắn đạn
    public GameObject bulletPrefab;     // prefab viên đạn
    public float attackCooldown = 1f;   // tốc độ bắn
    public float bulletSpeed = 12f;     // tốc độ đạn

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

        // 1. Lấy Rigidbody của mục tiêu (Giả sử enemy có Rigidbody để lấy vận tốc)
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 targetVelocity = Vector3.zero;
        if (targetRb != null)
        {
            targetVelocity = targetRb.linearVelocity; // Lấy vận tốc hiện tại của enemy
        }

        // 2. Tính toán vị trí dự đoán của mục tiêu
        // Khoảng cách giữa trụ và mục tiêu
        Vector3 displacement = target.position - firePoint.position;
        float distance = displacement.magnitude;

        // Thời gian để đạn bay tới mục tiêu (xấp xỉ)
        float timeToHit = distance / bulletSpeed;

        // Vị trí dự đoán: vị trí hiện tại + (vận tốc * thời gian bay)
        Vector3 predictedPosition = target.position + targetVelocity * timeToHit;

        // 3. Tính toán hướng bắn tới vị trí dự đoán
        Vector3 dir = (predictedPosition - firePoint.position).normalized;

        // 4. Tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));

        // 5. Tạo chuyển động
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // Sử dụng dir đã được tính toán dự đoán để bắn
        rb.linearVelocity = dir * bulletSpeed;
    }
}