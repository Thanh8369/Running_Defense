using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    //public PlayerArrowDama playerDamage;
    
    public AudioSource audioSource;
    public AudioClip attackClip;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //takedamage(damage)
            //GetComponent<EnemyHealth>().TakeDamage(playerDamage.damage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            PlayAttackSound();
        }
    }

    void PlayAttackSound()
    {
        // Only play if the previous sound has finished
        if (!audioSource.isPlaying)
        {
            audioSource.clip = attackClip;
            audioSource.Play();
        }
    }
}
