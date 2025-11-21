using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Transform target;
    private float damage;
    Vector3 direction;
    private Vector3 nonHomingDirection;

    [Header("Settings")]
    [SerializeField] private bool useHoming = true;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float heightOffset = 1f;

    public void Initialize(Transform target, float damage, Vector3 fireDirection)
    {
        this.target = target;
        this.damage = damage;

        if (target != null)
        {
            if (!useHoming)
            {
                nonHomingDirection = fireDirection.normalized;
            }
        }

        StartCoroutine(ReturnToPoolAfterLifetime());
    }

    void Update()
    {
        MoveProjectile();
    }

    void MoveProjectile()
    {
        if (useHoming && target != null)
        {
            Vector3 targetPos = target.position + Vector3.up * heightOffset;
            direction = (targetPos - transform.position).normalized;
        }
        else
        {
            direction = nonHomingDirection;
        }

        float distance = projectileSpeed * Time.deltaTime;
        transform.position += direction * distance;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.transform == target)
        {
            target.GetComponent<IDamageable>()?.TakeDamage(damage);
        }

        PoolManager.Instance.Return(gameObject);
    }

    private IEnumerator ReturnToPoolAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        PoolManager.Instance.Return(gameObject);
    }

}
