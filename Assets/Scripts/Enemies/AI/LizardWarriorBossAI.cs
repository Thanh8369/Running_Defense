using System;
using System.Collections.Generic;
using UnityEngine;

public class LizardWarriorAI : MeleeEnemyAI
{
    [Header("Boss Healing Settings")]
    [SerializeField] private float healCooldown = 15f;
    [SerializeField] private float healDuration = 3f;
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float defenseDamageReduction = 0.6f;
    [SerializeField] private float healTickInterval = 1f; // heal mỗi 1s

    [Header("Attack Variants")]
    [SerializeField] private List<LizardWarriorAttackVariant> attackVariants = new List<LizardWarriorAttackVariant>();

    private float lastHealTime = -999f;
    private float healStartTime;
    private float healTickTimer = 0f;

    private bool isHealing = false;
    private int currentAttackVariantId = 0;

    private EnemyHealth enemyHealth;
    private EnemyAnimation enemyAnimation;

    protected override void Start()
    {
        base.Start();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    protected override void SetupBT()
    {
        attackType = AttackType.Melee;

        rootNode = new BTSelector(new List<BTNode>
        {
            BuildHealSequence(),
            BuildPlayerAttackSequence(),
            BuildTowerAttackSequence()
        });
    }

    private BTSequence BuildHealSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => CanHeal()),
            new BTAction(() => EnterHealingStance())
        });
    }

    private BTSequence BuildPlayerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(player) <= stats.detectionRange 
                               && currentFocusTime <= 0 
                               && !isHealing
                               && !isHit),
            new BTSelector(new List<BTNode>
            {
                BuildAttackSequence(player),
                BuildMoveSequence(player)
            })
        });
    }

    private BTSequence BuildTowerAttackSequence()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => !isHealing && !isHit),
            new BTSelector(new List<BTNode>
            {
                BuildTowerAttackWithCooldown(),
                BuildMoveSequence(tower)
            })
        });
    }

    private BTSequence BuildAttackSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(target) <= stats.attackRange),
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => AttackTarget(target))
        });
    }

    private BTSequence BuildMoveSequence(Transform target)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTAction(() => RotateToTarget(target)),
            new BTAction(() => MoveToTarget(target))
        });
    }

    private BTSequence BuildTowerAttackWithCooldown()
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(tower) <= stats.attackRange),
            new BTAction(() => RotateToTarget(tower)),
            new BTAction(() =>
            {
                if (currentFocusTime <= 0f)
                    currentFocusTime = stats.attackCooldown;

                return AttackTarget(tower);
            })
        });
    }

    private bool CanHeal()
    {
        if (isHealing) return false;
        if (enemyHealth == null) return false;

        float timeSinceLastHeal = Time.time - lastHealTime;
        float hpPercent = enemyHealth.GetHealthPercent();

        return timeSinceLastHeal >= healCooldown && hpPercent < 0.8f;
    }

    private BTNode.NodeState EnterHealingStance()
    {
        if (isHealing) return BTNode.NodeState.Success;

        isHealing = true;
        healStartTime = Time.time;
        healTickTimer = 0f;
        lastHealTime = Time.time;

        enemyAnimation?.PlayHealAnimation(true);

        ExecuteHeal(); // Heal ngay lập tức

        return BTNode.NodeState.Success;
    }

    protected override void Update()
    {
        base.Update();

        if (isHealing)
        {
            float elapsedTime = Time.time - healStartTime;

            healTickTimer += Time.deltaTime;
            if (healTickTimer >= healTickInterval)
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

    private void ExitHealingStance()
    {
        isHealing = false;
        healTickTimer = 0;
        onMove?.Invoke(false);
        enemyAnimation?.PlayHealAnimation(false);
    }

    public void ExecuteHeal()
    {
        if (enemyHealth == null) return;

        float current = enemyHealth.GetCurrentHealth();
        float max = enemyHealth.GetMaxHealth();
        float newHealth = Mathf.Min(current + healAmount, max);

        enemyHealth.ForceSetHealth(newHealth);

        Debug.Log($"[LizardWarriorBoss] HealTick: {newHealth}/{max}");

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

        Debug.Log($"[LizardWarriorBoss] Attack variant {currentAttackVariantId} - Damage: {damage}");
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
}

[System.Serializable]
public class LizardWarriorAttackVariant
{
    public int id;
    public string animationTriggerName;
    public float damageAmount = 15f;
}
