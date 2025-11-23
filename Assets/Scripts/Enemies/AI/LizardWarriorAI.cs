using System;
using System.Collections.Generic;
using UnityEngine;

public class LizardWarriorAI : MeleeEnemyAI
{
    [SerializeField] private List<LizardWarriorAttackVariant> attackVariants = new List<LizardWarriorAttackVariant>();

    [Header("Boss Healing Settings")]
    [SerializeField] private float healCooldown = 15f;
    [SerializeField] private float healDuration = 3f;
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float healTick = 1f;
    [SerializeField] private float defenseDamageReduction = 0.6f;

    [Header("Buff Skill Settings")]
    [SerializeField] private float buffRadius = 10f;
    [SerializeField] private float buffMultiplier = 1.4f;
    [SerializeField] private float buffDuration = 5f;

    private float lastHealTime = -999f;
    private float healStartTime;
    private float healTickTimer = 0f;
    private bool isHealing = false;
    private int currentAttackVariantId = 0;

    private EnemyHealth enemyHealth;
    private EnemyAnimation enemyAnimation;

    private new void OnEnable()
    {
        FindFirstObjectByType<BossHealthUI>(FindObjectsInactive.Include)?.HandleBossSpawned(this);
    }

    protected override void Start()
    {
        base.Start();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    protected override List<BTNode> GetAdditionalBTNodes()
    {
        return new List<BTNode> { BuildHealSequence() };
    }

    protected override bool IsBlockedByAdditionalCondition()
    {
        return isHealing;
    }

    private BTSequence BuildHealSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => CanHeal()),
            new BTAction(() => EnterHealingStance())
        });
    }

    protected override void Update()
    {
        base.Update();

        if (isHealing)
        {
            float elapsedTime = Time.time - healStartTime;
            healTickTimer += Time.deltaTime;

            if (healTickTimer >= healTick)
            {
                healTickTimer = 0f;
                ExecuteHeal();
            }

            if (elapsedTime >= healDuration)
            {
                ExitHealingStance();
            }
        }
    }

    private bool CanHeal()
    {
        if (isHealing || enemyHealth == null) return false;

        float timeSinceLastHeal = Time.time - lastHealTime;
        float hpPercent = enemyHealth.GetHealthPercent();

        return timeSinceLastHeal >= healCooldown && hpPercent < 0.5f;
    }

    private BTNode.NodeState EnterHealingStance()
    {
        if (isHealing) return BTNode.NodeState.Success;

        isHealing = true;
        isAttacking = false;
        isRotate = false;
        isHit = false;
        healStartTime = Time.time;
        healTickTimer = 0f;
        lastHealTime = Time.time;

        enemyAnimation?.PlayHealAnimation(true);
        ExecuteHeal();

        return BTNode.NodeState.Success;
    }

    private void ExitHealingStance()
    {
        isHealing = false;
        isAttacking = false;
        isRotate = false;
        isHit = false;
        healTickTimer = 0;
        onMove?.Invoke(false, 0);
        enemyAnimation?.PlayHealAnimation(false);
    }

    public void ExecuteHeal()
    {
        if (enemyHealth == null) return;

        float current = enemyHealth.GetCurrentHealth();
        float max = enemyHealth.GetMaxHealth();
        float newHealth = Mathf.Min(current + healAmount, max);

        enemyHealth.Heal(newHealth);

        if (newHealth >= max)
            ExitHealingStance();
    }

    public void OnAttackAnimationSet(string triggerName)
    {
        foreach (var variant in attackVariants)
        {
            if (triggerName.Contains(variant.animationTriggerName))
            {
                currentAttackVariantId = variant.id;
                return;
            }
        }
        currentAttackVariantId = 0;
    }

    public new void DealDamageToTarget()
    {
        if (currentTarget == null) return;

        LizardWarriorAttackVariant currentVariant = GetCurrentAttackVariant();
        float damage = currentVariant != null ? currentVariant.damageAmount : stats.attackDamage;

        currentTarget.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    private LizardWarriorAttackVariant GetCurrentAttackVariant()
    {
        foreach (var variant in attackVariants)
        {
            if (variant.id == currentAttackVariantId)
                return variant;
        }
        return null;
    }

    public bool IsHealing() => isHealing;

    public float GetDamageReductionMultiplier()
    {
        return isHealing ? (1f - defenseDamageReduction) : 1f;
    }

    public void CastSpeedBuff()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, buffRadius);

        foreach (var hit in hits)
        {
            if (hit.transform == this.transform) continue;

            EnemyAI ai = hit.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.ApplyBuff(BuffType.Speed, buffMultiplier, buffDuration);
            }
        }
    }
}

[Serializable]
public class LizardWarriorAttackVariant
{
    public int id;
    public string animationTriggerName;
    public float damageAmount = 15f;
}
