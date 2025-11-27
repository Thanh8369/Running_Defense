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
        if (tower == null)
        {
            tower = FindObjectOfType<TowerArea>();
            if (tower == null)
                Debug.LogError("Không tìm thấy TowerArea trong scene!");
        }

     
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
        // Nếu đang khóa target khi attack → không đổi target
        if (lockedTarget != null)
            return;

        // Xóa enemy chết / null
        tower.enemyQueue.RemoveAll(e =>
            e == null ||
            !e.gameObject.activeInHierarchy ||
            (e.TryGetComponent<EnemyHealth>(out var h) && h.IsDead())
        );

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        // Nếu target hiện tại không còn hợp lệ → đổi target RANDOM
        if (target == null || !tower.enemyQueue.Contains(target))
        {
            int r = Random.Range(0, tower.enemyQueue.Count);
            target = tower.enemyQueue[r];
        }
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

        // Xoay mặt về target
        Vector3 aimDir = target.position - transform.position;
        aimDir.y = 0;

        if (aimDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(aimDir);

        // Lấy đạn từ pool
        GameObject bullet = PoolManager.Instance.Get(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Gán hướng bay
        Vector3 dir = (target.position - firePoint.position).normalized;
        bullet.GetComponent<Bullet>().Init(dir, bulletPrefab);
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