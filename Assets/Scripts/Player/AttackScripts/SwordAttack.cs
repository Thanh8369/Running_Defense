using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    //public PlayerArrowDama playerDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //takedamage(damage)
            //GetComponent<EnemyHealth>().TakeDamage(playerDamage.damage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            Debug.Log("hit");
        }
    }
}
