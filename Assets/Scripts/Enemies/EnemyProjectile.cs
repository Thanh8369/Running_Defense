using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float damage;
    private float speed;
    private Vector3 nonHomingDirection;

    [Header("Settings")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private bool useHoming = true;
    [SerializeField] private float heightOffset = 1f;
    [SerializeField] private GameObject hitEffectPrefab;

    public void Initialize(Transform target, float damage, float speed, Vector3 fireDirection)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;

        if (target != null)
        {
            if (!useHoming)
            {
                nonHomingDirection = fireDirection.normalized;
            }
        }
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        Vector3 direction;

        // HOMING
        if (useHoming && target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * heightOffset;
            direction = (targetPos - transform.position).normalized;
        }
        else
        {
            direction = nonHomingDirection;
        }

        float distance = speed * Time.deltaTime;

        transform.position += direction * distance;
        transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.transform == target)
        {
            HitTarget();
        }
        else if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        if (target != null)
        {
            var damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        SpawnHitEffect();
        Destroy(gameObject);
    }

    void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
