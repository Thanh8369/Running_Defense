using UnityEngine;

public class BulletTower : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float hitDistance = 0.3f;

    private float damage;
    private Transform target;

    private float ttl;
    private GameObject prefabOrigin;

    public void Init(Transform enemy, TowerRunStats data, GameObject prefab)
    {
        target = enemy;
        prefabOrigin = prefab;

        ttl = 0f;

        // Lấy damage từ TowerData
        damage = data.TotalAttackDamage;

        gameObject.SetActive(true);
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, target.position) <= hitDistance)
        {
            HitTarget();
        }

        ttl += Time.deltaTime;
        if (ttl >= lifeTime)
            ReturnToPool();
    }

    void HitTarget()
    {
        IDamageable dmg = target.GetComponent<IDamageable>();
        if (dmg == null) dmg = target.GetComponentInParent<IDamageable>();

        if (dmg != null)
            dmg.TakeDamage(damage);

        ReturnToPool();
    }

    void ReturnToPool()
    {
        ttl = 0f;
        PoolManager.Instance.Return(gameObject, prefabOrigin);
    }
}