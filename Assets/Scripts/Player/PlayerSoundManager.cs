using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioSource audioSource;   
    public AudioClip attackClip;      

    void Update()
    {
        // Example: press left mouse to attack
        if (Input.GetMouseButtonDown(2))
        {
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
