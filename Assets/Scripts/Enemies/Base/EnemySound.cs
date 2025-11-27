using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [Header("Single Audio Source")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip tauntingClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip getHitClip;

    private void Awake()
    {
        UISettings uiSettings = FindFirstObjectByType<UISettings>();
        if (uiSettings != null && _audioSource != null)
        {
            uiSettings.AddSfxSource(_audioSource);
        }
    }

    // ================== PLAY SOUNDS ==================
    public void PlayTauntingSound()
    {
        _audioSource.PlayOneShot(tauntingClip);
    }

    public void PlayDieSound()
    {
        _audioSource.PlayOneShot(dieClip);
    }

    public void PlayGetHitSound()
    {
        _audioSource.PlayOneShot(getHitClip);
    }
}
