using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useHoming = true;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float heightOffset = 1f;

    [Header("Poison Settings Per Projectile")]
    public PoisonDebuffConfig poisonConfig;

    [Header("Ice Slow Settings Per Projectile")]
    public SlowDebuffConfig iceSlowConfig;

    private Transform target;
    private float damage;
    private Vector3 direction;
    private Vector3 nonHomingDirection;
    private Vector3 originalScale;
    private bool isMoving = true;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialize(Transform target, float damage, Vector3 fireDirection)
    {
        this.target = target;
        this.damage = damage;
        transform.localScale = originalScale;

        // Non-homing direction
        if (target != null && !useHoming)
            nonHomingDirection = fireDirection.normalized;

        StartCoroutine(ReturnAfterLifetime());
    }

    private void Update()
    {
        if (isMoving)
            MoveProjectile();
    }

    private void MoveProjectile()
    {
        direction = GetDirection();
        transform.position += direction * projectileSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetDirection()
    {
        if (useHoming && target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * heightOffset;
            return (targetPos - transform.position).normalized;
        }

        return nonHomingDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageable>()?.TakeDamage(damage);

        // Poison
        if (poisonConfig != null && poisonConfig.enablePoison)
        {
            var poison = other.GetComponent<PoisonDebuff>();
            poison?.ApplyPoison(poisonConfig);
        }

        // Ice Slow
        if (iceSlowConfig != null && iceSlowConfig.enableIceSlow)
        {
            var ice = other.GetComponent<SlowDebuff>();
            ice?.ApplySlow(iceSlowConfig);
        }

        PoolManager.Instance.Return(gameObject);
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        PoolManager.Instance.Return(gameObject);
    }

    public void StopMoving() => isMoving = false;
    public void StartMoving() => isMoving = true;
}
