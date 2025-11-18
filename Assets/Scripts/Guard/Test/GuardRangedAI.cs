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

    // 🔥 Khóa target trong lúc attack
    private Transform lockedTarget = null;

    void Start()
    {
        animController = GetComponent<GuardAnimation>();
    }

    void Update()
    {
        // 🔥 1) Nếu đang attack → đứng yên tuyệt đối
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            return;
        }

        // 🔥 2) Nếu animation attack xong → mở khóa target
        if (lockedTarget != null && !animController.isAttacking)
            lockedTarget = null;

        UpdateTarget();

        // Không có enemy → về điểm gác
        if (target == null)
        {
            if (guardPoint != null)
                MoveTo(guardPoint.position);
            else
                animController.SetMoving(false);
            return;
        }

        // Enemy rời khỏi tower
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // 🔥 3) Giữ khoảng cách khi đuổi
        if (dist > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            animController.SetMoving(false);
        }

        // 🔥 4) Tấn công nếu trong tầm
        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    // ====================================================
    //                MOVE FUNCTION
    // ====================================================
    void MoveTo(Vector3 pos)
    {
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            return;
        }

        animController.SetMoving(true);

        // Xoay về hướng di chuyển
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

    // ====================================================
    //                 UPDATE TARGET
    // ====================================================
    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null);

        // 🔥 5) Nếu đang attack → KHÔNG đổi target
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

    // ====================================================
    //                     SHOOT
    // ====================================================
    void ShootAtTarget()
    {
        // Khóa target để AI không đổi target khi đang bắn
        lockedTarget = target;

        // Chạy animation
        animController.PlayAttack();
    }

    // ====================================================
    //   Animation Event gọi hàm này khi đến frame bắn
    // ====================================================
    public void OnShootEvent()
    {
        // Lấy target hiện tại, không dùng lockedTarget nữa
        if (target == null || firePoint == null) return;

        // Xoay mặt về enemy khi bắn
        Vector3 aimDir = target.position - transform.position;
        aimDir.y = 0;
        if (aimDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(aimDir);

        // Bắn đạn
        Vector3 dir = (target.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(dir);
    }

    // ====================================================
    //   Animation Event cuối clip gọi hàm này
    // ====================================================
   
}