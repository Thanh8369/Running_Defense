using UnityEngine;

public class BulletTower : MonoBehaviour
{
    [Header("Bullet flight")]
    public float speed = 20f;
    public float lifeTime = 3f;
    public float hitDistance = 0.3f;

    // runtime values (set từ Tower khi spawn)
    private float damage;
    private Transform target;
    private float ttlTimer;

    /// <summary>
    /// Init đạn: truyền target (Transform của enemy) và TowerRunStats của trụ bắn
    /// </summary>
    public void Init(Transform enemy, TowerRunStats towerStats)
    {
        target = enemy;
        if (towerStats != null)
        {
            damage = towerStats.TotalAttackDamage; // base + bonus
        }

        // reset thời gian tồn tại
        ttlTimer = 0f;

        // auto hủy sau lifeTime — nếu bạn dùng pool thì thay Destroy bằng ReturnToPool
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Hủy nếu target chết / inactive
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Destroy(gameObject);
            return;
        }

        // di chuyển về hướng target (luôn cập nhật vị trí target để bám theo)
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // xoay đầu mũi tên / đạn
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // kiểm tra va chạm bằng khoảng cách (nhanh và đơn giản)
        if (Vector3.Distance(transform.position, target.position) <= hitDistance)
        {
            HitTarget();
        }

        // an toàn: tăng timer (nếu bạn muốn tự hủy mà không dùng Destroy(...) trên Init)
        ttlTimer += Time.deltaTime;
        if (ttlTimer >= lifeTime)
        {
            // nếu đến time out mà chưa va chạm
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        if (target == null) return;

        // Lấy IDamageable ở object hoặc parent (phòng trường hợp collider ở child)
        IDamageable dmg = target.GetComponent<IDamageable>();
        if (dmg == null) dmg = target.GetComponentInParent<IDamageable>();

        if (dmg != null)
        {
            dmg.TakeDamage(damage);
        }
        else
        {
            // fallback: nếu không có IDamageable, thử tìm collider gần nhất
            Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (var c in cols)
            {
                var dd = c.GetComponentInParent<IDamageable>();
                if (dd != null)
                {
                    dd.TakeDamage(damage);
                    break;
                }
            }
        }

        // hủy đạn sau khi đánh (hoặc trả pool nếu dùng pool)
        Destroy(gameObject);
    }
}