using System.Collections;
using UnityEngine;

[System.Serializable]
public class PoisonDebuffConfig
{
    public bool enablePoison = false;
    public float damagePerTick = 2f;
    public float tickInterval = 1f;
    public float duration = 5f;
    public bool stack = false;
}

public class PoisonDebuff : MonoBehaviour
{
    public Material poisonMaterial;
    public float flashInterval = 0.2f;

    private Health health;
    private Coroutine effectRoutine;
    private Renderer[] renderers;
    private Material[][] originalMaterials;

    private void Awake()
    {
        health = GetComponent<Health>();
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
        }
    }

    public void ApplyPoison(PoisonDebuffConfig config)
    {
        if (!config.enablePoison || health == null || poisonMaterial == null) return;

        if (!config.stack && effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
            ResetMaterials();
        }

        effectRoutine = StartCoroutine(PoisonRoutine(config));
    }

    private IEnumerator PoisonRoutine(PoisonDebuffConfig config)
    {
        float timer = 0f, tickTimer = 0f, flashTimer = 0f;
        bool flashOn = false;

        while (timer < config.duration)
        {
            float dt = Time.deltaTime;
            timer += dt;
            tickTimer += dt;
            flashTimer += dt;

            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                flashOn = !flashOn;
                ApplyMaterialFlash(flashOn);
            }

            if (tickTimer >= config.tickInterval)
            {
                health.TakeDamage(config.damagePerTick);
                tickTimer = 0f;
            }

            yield return null;
        }

        ResetMaterials();
        effectRoutine = null;
    }

    private void ApplyMaterialFlash(bool flashOn)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            var mats = r.materials;
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] = flashOn ? poisonMaterial : originalMaterials[i][j];
            }
            r.materials = mats;
        }
    }

    private void ResetMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
    }
}
