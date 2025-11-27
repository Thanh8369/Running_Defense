using System.Collections;
using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [Header("Runtime Settings")]
    [SerializeField] private float checkInterval = 0.2f;
    [SerializeField] private float scaleUpTime = 0.3f;     // thời gian lớn lên
    [SerializeField] private float fadeOutTime = 0.4f;     // thời gian mờ đi
    [SerializeField] private float maxScale = 6f;          // scale cuối cùng

    private SlowDebuffConfig slowDebuffConfig;
    private Collider zoneCollider;
    private float lastCheckTime;

    private Material zoneMaterial;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider>();
        zoneMaterial = GetComponentInChildren<Renderer>().material;
    }

    private void OnEnable()
    {
        lastCheckTime = -999f;

        transform.localScale = Vector3.zero;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(ScaleAndFadeRoutine());
    }

    public void Initialize(SlowDebuffConfig config, float lifetime)
    {
        slowDebuffConfig = config;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(ScaleAndFadeRoutine(lifetime));
    }

    private IEnumerator ScaleAndFadeRoutine(float lifetime = 5f)
{
    // PHASE 1 — Scale up (X,Z), giữ nguyên Y
    float originalY = transform.localScale.y;

    float t = 0f;
    while (t < scaleUpTime)
    {
        t += Time.deltaTime;
        float k = t / scaleUpTime;

        Vector3 from = new Vector3(0f, originalY, 0f);
        Vector3 to   = new Vector3(maxScale, originalY, maxScale);

        transform.localScale = Vector3.Lerp(from, to, k);

        yield return null;
    }

    yield return new WaitForSeconds(lifetime - fadeOutTime);

    // PHASE 2 — Fade out
    Color startColor = zoneMaterial.color;
    float a0 = startColor.a;

    t = 0f;
    while (t < fadeOutTime)
    {
        t += Time.deltaTime;
        float k = t / fadeOutTime;

        Color c = startColor;
        c.a = Mathf.Lerp(a0, 0f, k);
        zoneMaterial.color = c;

        yield return null;
    }

    PoolManager.Instance.Return(gameObject);
}

    private void Update()
    {
        if (Time.time - lastCheckTime >= checkInterval)
        {
            ApplySlowToTargets();
            lastCheckTime = Time.time;
        }
    }

    private void ApplySlowToTargets()
    {
        Collider[] hits = Physics.OverlapBox(
            zoneCollider.bounds.center,
            zoneCollider.bounds.extents,
            transform.rotation,
            LayerMask.GetMask("Default")
        );

        foreach (var hit in hits)
        {
            SlowDebuff debuff = hit.GetComponent<SlowDebuff>();
            if (debuff != null)
                debuff.ApplySlow(slowDebuffConfig);
        }
    }
}
