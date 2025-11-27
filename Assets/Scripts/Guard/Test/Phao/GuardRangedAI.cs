using UnityEngine;

public class GuardRangedAI : MonoBehaviour
{
    public TowerArea tower;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float moveSpeed = 3f;
    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public float stopDistanceFromEnemy = 3f;

    private Transform target;
    private float nextAttack;

    private WheelRotation wheelController;

    void Start()
    {
        wheelController = GetComponent<WheelRotation>();

        if (tower == null)
        {
            tower = FindObjectOfType<TowerArea>();
            if (tower == null)
                Debug.LogError("Không tìm thấy TowerArea trong scene!");
        }
    }

    void Update()
    {
        UpdateTarget();

        if (target == null)
        {
            if (wheelController) wheelController.SetMoving(false);
            return;
        }

        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float distToEnemy = Vector3.Distance(transform.position, target.position);

        // -------------------------
        //  DI CHUYỂN GIỮ KHOẢNG CÁCH
        // -------------------------
        if (distToEnemy > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            if (wheelController) wheelController.SetMoving(false);

            // Xoay lập tức về target
            InstantRotateTo(target.position);

            // Bắn nếu gần đủ
            if (distToEnemy <= attackRange && Time.time >= nextAttack)
            {
                Shoot();
                nextAttack = Time.time + attackCooldown;
            }
        }
    }

    void MoveTo(Vector3 pos)
    {
        if (wheelController) wheelController.SetMoving(true);

        Vector3 flat = new Vector3(pos.x, transform.position.y, pos.z);

        // Xoay lập tức luôn khi di chuyển
        InstantRotateTo(flat);

        transform.position = Vector3.MoveTowards(
            transform.position,
            flat,
            moveSpeed * Time.deltaTime
        );
    }

    // -------------------------------------
    //  XOAY LẬP TỨC (Không mượt)
    // -------------------------------------
    void InstantRotateTo(Vector3 lookPos)
    {
        Vector3 dir = (lookPos - transform.position);
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    void UpdateTarget()
    {
        tower.enemyQueue.RemoveAll(e => e == null ||
            (e.TryGetComponent<EnemyHealth>(out var h) && h.IsDead()));

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    void Shoot()
    {
        if (firePoint == null || target == null)
            return;

        GameObject bullet = PoolManager.Instance.Get(bulletPrefab, firePoint.position, firePoint.rotation);

        BulletCanon bc = bullet.GetComponent<BulletCanon>();
        bc.Init(target, bulletPrefab);
    }
}