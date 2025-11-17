using UnityEngine;
using UnityEngine.AI;

public class GuardRangedAI : MonoBehaviour
{
    public TowerArea tower;
    public NavMeshAgent agent;
    public Transform guardPoint;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public float stopDistanceFromEnemy = 3f;

    private Transform target;
    private float nextAttack;

    private GuardAnimation animController;

    void Start()
    {
        animController = GetComponent<GuardAnimation>();
    }

    void Update()
    {
        if (animController.isAttacking)
        {
            agent.ResetPath();
            return;
        }
        UpdateTarget();

        if (target == null)
        {
            if (guardPoint != null)
                agent.SetDestination(guardPoint.position);
            return;
        }

        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > stopDistanceFromEnemy)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }

        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    void ShootAtTarget()
    {
        animController.PlayAttack(); // ★ Animation dùng chung

        Vector3 dir = (target.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
}