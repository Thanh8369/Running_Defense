using UnityEngine;
using UnityEngine.AI;

public class GuardRangedAI : MonoBehaviour
{
    public TowerArea tower;           // Trụ để kiểm soát phạm vi
    public NavMeshAgent agent;        // Agent điều khiển di chuyển
    public Transform guardPoint;      // Điểm đứng khi không có enemy
    public GameObject bulletPrefab;
    public Transform firePoint;       // Vị trí bắn trên lính
    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public float stopDistanceFromEnemy = 3f; // Khoảng cách lính giữ với enemy

    private Transform target;
    private float nextAttack;

    void Update()
    {
        UpdateTarget();

        if (target == null)
        {
            // Không có enemy → quay về guardPoint
            if (guardPoint != null && agent != null)
            {
                agent.SetDestination(guardPoint.position);
            }
            return;
        }

        // Enemy còn trong phạm vi tower
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // Di chuyển gần enemy nhưng không vượt quá stopDistanceFromEnemy
        if (dist > stopDistanceFromEnemy)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.SetDestination(transform.position); // đứng yên bắn
        }

        // Bắn nếu trong tầm
        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    void UpdateTarget()
    {
        // Cleanup enemy null
        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
        {
            target = tower.enemyQueue[0];
        }
    }

    void ShootAtTarget()
    {
        if (bulletPrefab == null || firePoint == null || target == null)
            return;

        Vector3 dir = (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
}