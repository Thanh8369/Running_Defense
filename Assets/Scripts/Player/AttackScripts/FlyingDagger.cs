using UnityEngine;

public class FlyingDagger : MonoBehaviour
{
    Rigidbody rb;
    public PlayerArrowDama playerDamage;
    public float lifeTime = 2f;
    private float timer = 0f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(rb.linearVelocity);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, target, Time.fixedDeltaTime * 10f));
        }
        
    }

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifeTime);
    }
    void ReturnToPool()
    {
        DaggerObjectPool.Instance.Return(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            
            //GetComponent<EnemyHealth>().TakeDamage(playerDamage.damage);
            damageable.TakeDamage(playerDamage.damage);
            ReturnToPool();
            //Debug.Log("hit");
        }
    }
}
