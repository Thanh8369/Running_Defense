using UnityEngine;

public class GuardAiRange : MonoBehaviour
{
    public TowerArea tower;

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

    // Target bị khóa trong lúc attack
    private Transform lockedTarget = null;

    void Start()
    {
        animController = GetComponent<GuardAnimation>();
    }

    void Update()
    {
        // 1) Nếu đang attack → đứng yên
        if (animController.isAttacking)
        {
            animController.SetMoving(false);
            return;
        }

        // Attack xong → mở khóa target
        if (lockedTarget != null && !animController.isAttacking)
            lockedTarget = null;

        UpdateTarget();

        // 2) KHÔNG CÓ TARGET → đứng im chờ
        if (target == null)
        {
            animController.SetMoving(false);
            return;
        }

        // Nếu target không còn trong danh sách tower
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);

        // 3) DI CHUYỂN → giữ khoảng cách
        if (dist > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            animController.SetMoving(false);
        }

        // 4) ATTACK
        if (dist <= attackRange && Time.time >= nextAttack)
        {
            ShootAtTarget();
            nextAttack = Time.time + attackCooldown;
        }
    }

    // MOVE
    void MoveTo(Vector3 pos)
    {
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

    // UPDATE TARGET
    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null);

        // Đang attack → không đổi target
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

    // ATTACK
    void ShootAtTarget()
    {
        lockedTarget = target;
        animController.PlayAttack();
    }

    // Animation Event – bắn tại frame chính xác
    public void OnShootEvent()
    {
        if (target == null || firePoint == null)
            return;

        Vector3 aimDir = target.position - transform.position;
        aimDir.y = 0;

        if (aimDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(aimDir);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection((target.position - firePoint.position).normalized);
    }

    // Animation Event – xoay theo target khi đang kéo cung
    public void OnAttackRotate()
    {
        if (lockedTarget == null) return;

        Vector3 dir = lockedTarget.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }
}