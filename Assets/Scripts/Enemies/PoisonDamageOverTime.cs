using System.Collections;
using UnityEngine;

[System.Serializable]
public class PoisonConfig
{
    public bool enablePoison = false;
    public float poisonDamagePerTick = 2f;
    public float tickInterval = 1f;
    public float poisonDuration = 5f;
    public bool stackPoison = false;
}

public class PoisonDamageOverTime : MonoBehaviour
{
    public Material poisonMaterial;
    public float flashInterval = 0.2f;

    private Health health;
    private Coroutine poisonRoutine;

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

    public void ApplyPoison(PoisonConfig config)
    {
        if (health == null || config == null || !config.enablePoison)
            return;

        if (!config.stackPoison && poisonRoutine != null)
        {
            StopCoroutine(poisonRoutine);
            ResetMaterials();
        }

        poisonRoutine = StartCoroutine(PoisonRoutine(config));
    }

    private IEnumerator PoisonRoutine(PoisonConfig config)
    {
        float timer = 0f;
        float tickTimer = 0f;      // để kiểm soát tick damage
        float flashTimer = 0f;
        bool flashOn = false;

        while (timer < config.poisonDuration)
        {
            // Cập nhật timer
            float delta = Time.deltaTime;
            timer += delta;
            tickTimer += delta;
            flashTimer += delta;

            // Nhấp nháy material
            if (flashTimer >= flashInterval)
            {
                flashTimer = 0f;
                flashOn = !flashOn;
                ApplyPoisonMaterial(flashOn, poisonMaterial);
            }

            // Trừ damage khi tickTimer đạt tickInterval
            if (tickTimer >= config.tickInterval)
            {
                health.TakeDamage(config.poisonDamagePerTick);
                tickTimer = 0f;
            }

            yield return null;
        }

        ResetMaterials();
        poisonRoutine = null;
    }


    private void ApplyPoisonMaterial(bool flashOn, Material poisonMaterial)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = new Material[renderers[i].materials.Length];
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] = flashOn && poisonMaterial != null ? poisonMaterial : originalMaterials[i][j];
            }
            renderers[i].materials = mats;
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
