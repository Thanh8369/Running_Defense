using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public TowerArea tower;
    public NavMeshAgent agent;
    public Transform guardPoint;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private Transform target;
    private float nextAttack;

    void Update()
    {
        UpdateTarget();

        // Nếu không có địch trong phạm vi → quay về trụ
        if (target == null)
        {
            ReturnToGuardPoint();
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // Nếu địch vẫn đang trong vùng trụ
        bool enemyStillInRange = tower.enemyQueue.Contains(target);

        if (!enemyStillInRange)
        {
            // Enemy ra khỏi vùng → bỏ target
            target = null;
            return;
        }

        // Enemy trong phạm vi → xử lý
        if (dist > attackRange)
        {
            // Chỉ chạy đến địch nếu vị trí địch cũng nằm trong vùng trụ
            agent.SetDestination(target.position);
        }
        else
        {
            agent.SetDestination(transform.position); // đứng lại
            Attack();
        }
    }

    void UpdateTarget()
    {
        // Xoá target null
        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null)
        {
            target = tower.enemyQueue[0];
            return;
        }

        if (!tower.enemyQueue.Contains(target))
        {
            target = tower.enemyQueue[0]; // chuyển sang enemy kế tiếp
        }
    }

    void ReturnToGuardPoint()
    {
        if (guardPoint != null)
            agent.SetDestination(guardPoint.position);
    }

    void Attack()
    {
        if (Time.time >= nextAttack)
        {
            Debug.Log("Guard Attack!");
            nextAttack = Time.time + attackCooldown;
        }
    }
}