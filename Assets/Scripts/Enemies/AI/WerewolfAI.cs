using System;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfAI : MeleeEnemyAI
{
    [Header("Attack Variants")]
    [SerializeField] private string troopAttackTrigger = "Attack1";
    [SerializeField] private string towerAttackTrigger = "Attack2";

    private string currentAttackTrigger = "";
    private EnemyAnimation enemyAnimation;
    private Animator animator;

    protected override void Start()
    {
        base.Start();
        enemyAnimation = GetComponent<EnemyAnimation>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        // Nếu đang tấn công tower nhưng troop xuất hiện gần, reset animation
        if (currentAttackTrigger == towerAttackTrigger && nearestTroop != null)
        {
            float d = Vector3.Distance(transform.position, nearestTroop.position);
            if (d <= stats.detectionRange)
            {
                enemyAnimation.OnAttackEnd();
                animator.Play("IdleBattle", 0, 0f);
            }
        }
    }

    protected override void SetupBT()
    {
        var nodes = new List<BTNode>();

        var additionalNodes = GetAdditionalBTNodes();
        if (additionalNodes != null)
            nodes.AddRange(additionalNodes);

        // Werewolf ưu tiên: Troop > Tower
        nodes.Add(BuildTargetSequence(
            () => nearestTroop,
            () => nearestTroop != null && DistanceToTarget(nearestTroop) <= stats.detectionRange,
            troopAttackTrigger
        ));

        nodes.Add(BuildTargetSequence(
            () => tower,
            () => true,
            towerAttackTrigger
        ));

        rootNode = new BTSelector(nodes);
    }

    protected override List<BTNode> GetAdditionalBTNodes()
    {
        return null;
    }

    private BTSequence BuildTargetSequence(
        Func<Transform> getTarget,
        Func<bool> condition,
        string attackTrigger)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => condition() && !IsBlockedByAdditionalCondition()),
            new BTSelector(new List<BTNode>
            {
                BuildAttackSequence(getTarget, attackTrigger),
                BuildMoveSequence(getTarget)
            })
        });
    }

    private BTSequence BuildAttackSequence(Func<Transform> getTarget, string attackTrigger)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTCondition(() => DistanceToTarget(getTarget()) <= stats.attackRange),
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() =>
            {
                currentAttackTrigger = attackTrigger;
                return AttackTarget(getTarget());
            })
        });
    }

    private BTSequence BuildMoveSequence(Func<Transform> getTarget)
    {
        return new BTSequence(new List<BTNode>
        {
            new BTAction(() => RotateToTarget(getTarget())),
            new BTAction(() => MoveToTarget(getTarget()))
        });
    }

    protected override bool IsBlockedByAdditionalCondition() => false;

    public string GetCurrentAttackTrigger() => currentAttackTrigger;

    public void OnAttackAnimationSet(string triggerName)
    {
        if (stats.attackVariants.Exists(v => v.triggerName == triggerName))
            currentAttackTrigger = triggerName;
    }

    public new void DealDamageToTarget()
    {
        if (currentTarget == null) return;

        var variant = GetCurrentAttackVariant();
        float damage = variant != null ? variant.damageAmount : stats.attackDamage;

        currentTarget.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    private AttackVariant GetCurrentAttackVariant()
    {
        foreach (var variant in stats.attackVariants)
        {
            if (variant.triggerName == currentAttackTrigger)
                return variant;
        }
        return null;
    }
}
