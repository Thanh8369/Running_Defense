using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    //ArrowDamage
    public PlayerRunStats playerDamage;
    public float lifeTime = 1f;
    private float timer = 0f;

    void OnEnable()
    {
        timer = 0f;
        Destroy(gameObject, lifeTime);
    }

    //void Update()
    //{
    //    //timer += Time.deltaTime;
    //    //if (timer >= lifeTime)
    //    //{
    //    //    ArrowObjectPool.Instance.ReturnObject(gameObject);
    //    //}
    //}

    private void OnTriggerEnter(Collider other)
    {
        playerDamage = FindAnyObjectByType<PlayerRunStats>();
        // Không cần check tag nếu tất cả enemy đều có IDamageable
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(playerDamage.TotalAttackDamage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            //Debug.Log("hit");
            Destroy(gameObject);
        }
    }
}
