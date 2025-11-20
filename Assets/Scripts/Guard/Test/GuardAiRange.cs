using UnityEngine;

public class GuardAiRange : MonoBehaviour
{
    public TowerArea tower;
    public Transform guardPoint;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float moveSpeed = 3f;
    public float rotateSpeed = 10f;
    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public float stopDistanceFromEnemy = 3f;

    private Transform target;
    private float nextAttack;

    private GuardAnimation animController;

    // Target bị khóa trong lúc attack → không đổi target
    private Transform lockedTarget = null;

    void Start()
    {
        animController = GetComponent<GuardAnimation>();
    }

    void Update()
    {
        // -----------------------------------------
        // 1) ĐANG ATTACK → ĐỨNG YÊN HOÀN TOÀN
        // -----------------------------------------
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            return;
        }

        // Attack kết thúc → mở khóa target
        if (lockedTarget != null && !animController.isAttacking)
            lockedTarget = null;

        UpdateTarget();

        // -----------------------------------------
        // 2) KHÔNG CÓ TARGET → VỀ GUARD POINT
        // -----------------------------------------
        if (target == null)
        {
            if (guardPoint != null)
            {
                float distToGuard = Vector3.Distance(transform.position, guardPoint.position);

                // Nếu đã về tới nơi → Idle
                if (distToGuard <= 0.1f)
                {
                    animController.SetMoving(false); // <-- QUAN TRỌNG
                }
                else
                {
                    MoveTo(guardPoint.position);
                }
            }
            else
            {
                animController.SetMoving(false);
            }
            return;
        }

        // -----------------------------------------
        // 3) TARGET RỜI KHỎI TOWER → BỎ TARGET
        // -----------------------------------------
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // -----------------------------------------
        // 4) MOVE GIỮ KHOẢNG CÁCH
        // -----------------------------------------
        if (dist > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            animController.SetMoving(false);
        }

        // -----------------------------------------
        // 5) ATTACK NẾU TRONG TẦM
        // -----------------------------------------
        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    // ============================================
    // MOVE FUNCTION
    // ============================================
    void MoveTo(Vector3 pos)
    {
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            return;
        }

        animController.SetMoving(true);

        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotateSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            pos,
            moveSpeed * Time.deltaTime
        );
    }

    // ============================================
    // UPDATE TARGET
    // ============================================
    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null);

        // đang tấn công → KHÔNG đổi target
        if (lockedTarget != null)
            return;

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    // ============================================
    // ATTACK
    // ============================================
    void ShootAtTarget()
    {
        lockedTarget = target; // khóa target
        animController.PlayAttack();
    }

    // ============================================
    // được gọi bởi Animation Event (giữa animation)
    // ============================================
    public void OnShootEvent()
    {
        if (target == null || firePoint == null)
            return;

        // xoay mặt khi bắn
        Vector3 aimDir = target.position - transform.position;
        aimDir.y = 0;

        if (aimDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(aimDir);

        // tạo đạn
        Vector3 dir = (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
    public void OnAttackRotate()
    {
        if (lockedTarget == null) return;

        Vector3 dir = lockedTarget.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}