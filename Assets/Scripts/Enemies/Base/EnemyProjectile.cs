using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useHoming = true;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float heightOffset = 1f;

    [Header("Bomb Settings")]
    public bool isBomb = false;
    public GameObject bombFragmentPrefab;
    public int fragmentCount = 8;   

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

        if (!useHoming)
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
        Vector3 dir = GetDirection();

        if (dir == Vector3.zero)
            return;

        transform.position += dir * projectileSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private Vector3 GetDirection()
    {
        if (!useHoming)
            return nonHomingDirection;

        if (target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * heightOffset;
            return (targetPos - transform.position).normalized;
        }

        return nonHomingDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageable>()?.TakeDamage(damage);

        if (poisonConfig != null && poisonConfig.enablePoison)
            other.GetComponent<PoisonDebuff>()?.ApplyPoison(poisonConfig);

        if (iceSlowConfig != null && iceSlowConfig.enableSlow)
            other.GetComponent<SlowDebuff>()?.ApplySlow(iceSlowConfig);

        if (isBomb)
            Explode();

        PoolManager.Instance.Return(gameObject);
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        if (isBomb)
            Explode();

        PoolManager.Instance.Return(gameObject);
    }

    private void Explode()
    {
        if (bombFragmentPrefab == null) return;

        float angleStep = 360f / fragmentCount;
        Vector3 pos = transform.position;

        for (int i = 0; i < fragmentCount; i++)
        {
            float angle = angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            GameObject frag = PoolManager.Instance.Get(
                bombFragmentPrefab,
                pos,
                Quaternion.LookRotation(dir)
            );

            EnemyProjectile fp = frag.GetComponent<EnemyProjectile>();
            if (fp != null)
            {
                // Fragment KHÔNG BAO GIỜ homing hoặc là bomb
                fp.useHoming = false;
                fp.isBomb = false;
                fp.bombFragmentPrefab = null;
                fp.fragmentCount = 0;

                // Gán hướng
                fp.nonHomingDirection = dir;

                // Damage giảm
                fp.Initialize(null, damage * 0.8f, dir);
            }
        }
    }

    public void StopMoving() => isMoving = false;
    public void StartMoving() => isMoving = true;
}
