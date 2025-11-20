using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    //ArrowDamage
    public float lifeTime = 3f;
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
        if (other.CompareTag("Enemy"))
        {
            //takedamage(damage)
            ArrowObjectPool.Instance.ReturnObject(gameObject);
        }
    }
}
