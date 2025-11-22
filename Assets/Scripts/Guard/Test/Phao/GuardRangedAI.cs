using UnityEngine;

public class GuardRangedAI : MonoBehaviour
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

        // ---------------------------------------------------------
        // ❌ KHÔNG CÓ TARGET → ĐỨNG YÊN (KHÔNG TRỞ VỀ GUARD POINT)
        // ---------------------------------------------------------
        if (target == null)
        {
            if (wheelController) wheelController.SetMoving(false);
            return;
        }

        // enemy rời tower
        if (!tower.enemyQueue.Contains(target))
        {
            target = null;
            return;
        }

        float distToEnemy = Vector3.Distance(transform.position, target.position);

        // ---------------------------------------------------------
        //  MOVE GIỮ KHOẢNG CÁCH
        // ---------------------------------------------------------
        if (distToEnemy > stopDistanceFromEnemy)
        {
            MoveTo(target.position);
        }
        else
        {
            if (wheelController) wheelController.SetMoving(false);
        }

        // ---------------------------------------------------------
        //  BẮN
        // ---------------------------------------------------------
        if (distToEnemy <= attackRange && Time.time >= nextAttack)
        {
            Shoot();
            nextAttack = Time.time + attackCooldown;
        }
    }

    void MoveTo(Vector3 pos)
    {
        if (wheelController) wheelController.SetMoving(true);

        Vector3 flat = new Vector3(pos.x, transform.position.y, pos.z);
        Vector3 dir = (flat - transform.position).normalized;

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
            flat,
            moveSpeed * Time.deltaTime
        );
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

    void Shoot()
    {
        if (firePoint == null || target == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        BulletCanon parabola = bullet.GetComponent<BulletCanon>();
        parabola.Init(target.position);
    }
}