using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool useHoming = true;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float heightOffset = 1f;

    private Transform target;
    private float damage;
    private Vector3 direction;
    private Vector3 nonHomingDirection;
    private bool isMoving = true;

    public void Initialize(Transform target, float damage, Vector3 fireDirection)
    {
        this.target = target;
        this.damage = damage;
        transform.localScale = Vector3.one;

        if (target != null && !useHoming)
            nonHomingDirection = fireDirection.normalized;

        StartCoroutine(ReturnToPoolAfterLifetime());
    }

    public void StopMoving() => isMoving = false;
    public void StartMoving() => isMoving = true;

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
        // if (target != null && other.transform == target)
        other.GetComponent<IDamageable>()?.TakeDamage(damage);

        PoolManager.Instance.Return(gameObject);
    }

    private IEnumerator ReturnToPoolAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        PoolManager.Instance.Return(gameObject);
    }
}
