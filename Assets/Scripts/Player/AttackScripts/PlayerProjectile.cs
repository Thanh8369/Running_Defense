using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    //ArrowDamage
    public PlayerRunStats playerDamage;
    public float lifeTime = 3f;
    private float timer = 0f;

    private void Awake()
    {
        playerDamage = FindAnyObjectByType<PlayerRunStats>();
    }

    void OnEnable()
    {
        timer = 0f;
        Destroy(gameObject, lifeTime);
    }

    //void Update()
    //{
    //    timer += Time.deltaTime;
    //    if (timer >= lifeTime)
    //    {
    //        //ArrowObjectPool.Instance.ReturnObject(gameObject);
    //        Destroy(gameObject, lifeTime);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        // Không cần check tag nếu tất cả enemy đều có IDamageable
        if (other.TryGetComponent<EnemyHealth>(out var damageable))
        {
            damageable.TakeDamage(playerDamage.TotalAttackDamage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            Debug.Log("hit");
            Destroy(gameObject);
        }
    }
}
