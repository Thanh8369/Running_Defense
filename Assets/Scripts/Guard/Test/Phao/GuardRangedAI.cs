using UnityEngine;

public class GuardRangedAI : MonoBehaviour
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
    private WheelRotation wheelController;

    private Transform lockedTarget = null;

    void Start()
    {
        animController = GetComponent<GuardAnimation>();
        wheelController = GetComponent<WheelRotation>();
    }

    void Update()
    {
        // 1) Đang Attack → đứng yên
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            wheelController.SetMoving(false);
            return;
        }

        // 2) Attack xong → mở khóa target
        if (lockedTarget != null && !animController.isAttacking)
            lockedTarget = null;

        UpdateTarget();

        // 3) Không có target → về guardPoint
        if (target == null)
        {
            if (guardPoint != null)
                MoveTo(guardPoint.position);
            else
            {
                animController.SetMoving(false);
                wheelController.SetMoving(false);
            }
            return;
        }

        // 4) Enemy rời tower → bỏ target
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // 5) Move nhưng giữ khoảng cách
        if (dist > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            animController.SetMoving(false);
            wheelController.SetMoving(false);
        }

        // 6) Attack khi trong tầm
        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    // =======================
    // MOVE FUNCTION
    // =======================
    void MoveTo(Vector3 pos)
    {
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            wheelController.SetMoving(false);
            return;
        }

        animController.SetMoving(true);
        wheelController.SetMoving(true);

        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0;

        // Xoay mượt
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotateSpeed * Time.deltaTime
            );
        }

        // Di chuyển
        transform.position = Vector3.MoveTowards(
            transform.position,
            pos,
            moveSpeed * Time.deltaTime
        );
    }

    // =======================
    // UPDATE TARGET
    // =======================
    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null);

        if (lockedTarget != null) return;   // đang attack thì KHÔNG đổi target

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    // =======================
    // ATTACK
    // =======================
    void ShootAtTarget()
    {
        lockedTarget = target;
        animController.PlayAttack();
    }

    // Animation Event → Bắn tại đúng frame
    public void OnShootEvent()
    {
        if (target == null || firePoint == null) return;

        // Xoay mặt về enemy
        Vector3 aimDir = target.position - transform.position;
        aimDir.y = 0;
        if (aimDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(aimDir);

        // Tạo đạn
        Vector3 dir = (target.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
}