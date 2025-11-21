using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 50f;    // Damage bạn muốn

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
        // Kiểm tra có IDamageable hay không
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);   // 🔥 Gây dame đúng hệ thống
            Destroy(gameObject);
            return;
        }

        // Nếu không có IDamageable nhưng có tag Enemy → fallback
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy FOUND but missing IDamageable → " + other.name);
            Destroy(gameObject);
        }
    }
}