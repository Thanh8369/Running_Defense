using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 50f;

    private Vector3 direction;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tìm IDamageable ở cả object và parent (GIẢI QUYẾT COLLIDER Ở CHILD)
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null)
            dmg = other.GetComponentInParent<IDamageable>();

        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Nếu enemy có tag nhưng không có script health
        if (other.CompareTag("Enemy"))
        {
            Debug.LogWarning("Enemy FOUND but missing IDamageable: " + other.name);
            Destroy(gameObject);
        }
    }
}