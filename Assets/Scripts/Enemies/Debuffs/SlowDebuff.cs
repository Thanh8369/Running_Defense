using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class SlowDebuffConfig
{
    public bool enableSlow = false;
    [Range(0f, 1f)] public float slowAmount = 0.5f;
    public float slowDuration = 3f;
    public bool stackSlow = false;
}

public class SlowDebuff : MonoBehaviour
{
    public GameObject iceVFXPrefab;

    private NavMeshAgent agent;
    private GameObject currentVFX;
    private bool isSlowed = false;
    private float originalSpeed;
    private float finalSpeed;
    private float endTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            originalSpeed = agent.speed;
    }

    private void Update()
    {
        if (isSlowed && Time.time >= endTime)
            RemoveSlow();
    }

    public void ApplySlow(SlowDebuffConfig config)
    {
        if (config == null || !config.enableSlow || agent == null)
            return;

        float targetSpeed = originalSpeed * (1f - Mathf.Clamp01(config.slowAmount));

        if (!isSlowed)
        {
            isSlowed = true;
            finalSpeed = targetSpeed;
            agent.speed = finalSpeed;
            PlayVFX();
        }
        else
        {
            if (config.stackSlow)
            {
                finalSpeed *= 1f - Mathf.Clamp01(config.slowAmount);
                agent.speed = finalSpeed;
            }
        }

        endTime = Time.time + config.slowDuration;
    }

    private void RemoveSlow()
    {
        isSlowed = false;
        agent.speed = originalSpeed;
        finalSpeed = originalSpeed;
        StopVFX();
    }

    private void PlayVFX()
    {
        if (iceVFXPrefab == null || currentVFX != null) return;

        currentVFX = PoolManager.Instance.Get(iceVFXPrefab, transform.position, Quaternion.identity);
        currentVFX.transform.SetParent(transform);
    }

    private void StopVFX()
    {
        if (currentVFX != null)
        {
            PoolManager.Instance.Return(currentVFX);
            currentVFX = null;
        }
    }
}
