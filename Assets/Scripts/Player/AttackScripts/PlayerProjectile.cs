using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    //ArrowDamage
    public PlayerArrowDama playerDamage;
    public float lifeTime = 1f;
    private float timer = 0f;

    void OnEnable()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ArrowObjectPool.Instance.ReturnObject(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Không cần check tag nếu tất cả enemy đều có IDamageable
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(playerDamage.damage);
            ArrowObjectPool.Instance.ReturnObject(gameObject);
            //Debug.Log("hit");
        }
    }
}
