using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 50f;

    private Vector3 direction;
    private float timer;
    private GameObject prefabOrigin; // prefab gốc để trả pool

    // Set direction + prefab gốc
    public void Init(Vector3 dir, GameObject prefab)
    {
        direction = dir.normalized;
        prefabOrigin = prefab;

        timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (direction == Vector3.zero) return;

        transform.position += direction * speed * Time.deltaTime;

        // Khóa xoay
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);
        if (flatDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(flatDir);

        timer += Time.deltaTime;
        if (timer >= lifeTime)
            ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == transform.parent) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null)
            dmg = other.GetComponentInParent<IDamageable>();

        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        if (other.CompareTag("Enemy"))
            ReturnToPool();
    }

    void ReturnToPool()
    {
        direction = Vector3.zero;
        timer = 0f;

        if (PoolManager.Instance != null)
            PoolManager.Instance.Return(gameObject, prefabOrigin);
        else
            gameObject.SetActive(false);
    }
}