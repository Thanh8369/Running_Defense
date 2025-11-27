using UnityEngine;
using System.Collections;

public class SummonCircle : MonoBehaviour
{
    private GameObject minionPrefab;
    private Vector3 spawnPosition;

    [Header("Scale Settings")]
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float scaleDownDuration = 0.25f;

    [Header("Summon Timing")]
    [SerializeField] private float spawnDelayAfterScaleUp = 0.1f;

    private Coroutine summonRoutine;

    public void Initialize(GameObject minion, Vector3 pos)
    {
        minionPrefab = minion;
        spawnPosition = pos;

        // Reset scale X/Z, giữ Y nguyên
        Vector3 local = transform.localScale;
        transform.localScale = new Vector3(startScale, local.y, startScale);

        if (summonRoutine != null)
            StopCoroutine(summonRoutine);

        summonRoutine = StartCoroutine(SummonSequence());
    }

    private IEnumerator SummonSequence()
    {
        // Scale Up X/Z
        yield return ScaleRoutine(startScale, maxScale, scaleUpDuration);

        // Delay nhỏ trước khi spawn minion
        yield return new WaitForSeconds(spawnDelayAfterScaleUp);

        // Spawn minion
        PoolManager.Instance.Get(minionPrefab, spawnPosition, Quaternion.identity);

        // Scale Down X/Z
        yield return ScaleRoutine(maxScale, 0f, scaleDownDuration);

        // Return về Pool
        PoolManager.Instance.Return(gameObject, gameObject);
    }

    private IEnumerator ScaleRoutine(float from, float to, float duration)
    {
        float t = 0f;
        float originalY = transform.localScale.y;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            float scale = Mathf.Lerp(from, to, lerp);

            transform.localScale = new Vector3(scale, originalY, scale);

            yield return null;
        }
    }
}
