using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public float damage = 50f;

    private Vector3 direction;

    /// <summary>
    /// Gán hướng bay cho viên đạn
    /// </summary>
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;

        // Tự hủy sau time sống
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Nếu chưa có direction → không làm gì
        if (direction == Vector3.zero) return;

        // Bay thẳng
        transform.position += direction * speed * Time.deltaTime;

        // KHÓA xoay theo trục Y để đầu mũi tên không bị nghiêng
        Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);
        if (flatDir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(flatDir);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ngăn tự bắn vào mình hoặc tower
        if (other.transform == transform.parent) return;

        // Lấy IDamageable ở object hoặc parent
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null)
            dmg = other.GetComponentInParent<IDamageable>();

        // Nếu có script damage → gây damage
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Enemy có tag nhưng thiếu script → cảnh báo
        if (other.CompareTag("Enemy"))
        {
            Debug.LogWarning("Enemy FOUND but missing IDamageable: " + other.name);
            Destroy(gameObject);
        }
    }
}