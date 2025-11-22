using UnityEngine;

public class FlyingDagger : MonoBehaviour
{
    Rigidbody rb;
    public PlayerArrowDama playerDamage;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            GetComponent<EnemyHealth>().TakeDamage(playerDamage.damage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            //Debug.Log("hit");
        }
    }
}
