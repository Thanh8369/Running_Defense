using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAnimationConfig animationConfig;

    private Animator animator;
    private EnemyAI enemyAI;
    private CyclopsAI cyclopsAI;
    private EvilMageAI evilMageAI;
    private OrcAI orcAI;
    private LizardWarriorAI lizardWarriorAI;
    private EnemyHealth enemyHealth;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int GetHit = Animator.StringToHash("GetHit");
    private static readonly int DefendGetHit = Animator.StringToHash("DefendGetHit");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int IsHeal = Animator.StringToHash("IsHealing");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();
        cyclopsAI = GetComponent<CyclopsAI>();
        evilMageAI = GetComponent<EvilMageAI>();
        orcAI = GetComponent<OrcAI>();
        lizardWarriorAI = GetComponent<LizardWarriorAI>();
    }

    private void OnEnable()
    {
        if (enemyAI != null)
        {
            enemyAI.onMove += PlayWalkAnimation;
            enemyAI.onAttack += PlayAttackAnimation;
        }

        if (enemyHealth != null)
        {
            enemyHealth.onHit += PlayHitAnimation;
            enemyHealth.onDie += PlayDeathAnimation;
        }
    }

    private void OnDisable()
    {
        if (enemyAI != null)
        {
            enemyAI.onMove -= PlayWalkAnimation;
            enemyAI.onAttack -= PlayAttackAnimation;
        }

        if (enemyHealth != null)
        {
            enemyHealth.onHit -= PlayHitAnimation;
            enemyHealth.onDie -= PlayDeathAnimation;
        }
    }

    private void PlayWalkAnimation(bool isWalking)
    {
        animator.SetBool(IsWalking, isWalking);
    }

    private void PlayAttackAnimation(string atkType)
    {
        if (animationConfig == null) return;

        if (atkType == "defend")
        {
            animator.SetTrigger("Defend");
            return;
        }

        EnemyAnimationData anim = atkType == "ranged"
            ? animationConfig.GetRandomAttack(animationConfig.rangedAttacks)
            : animationConfig.GetRandomAttack(animationConfig.meleeAttacks);

        if (anim != null)
        {
            animator.SetTrigger(anim.triggerName);

            if (atkType == "ranged")
            {
                cyclopsAI?.OnAttackAnimationSet(anim.triggerName);
                evilMageAI?.OnAttackAnimationSet(anim.triggerName);
            }

            if (atkType == "melee")
            {
                orcAI?.OnAttackAnimationSet(anim.triggerName);
                lizardWarriorAI?.OnAttackAnimationSet(anim.triggerName);
            }
        }
    }

    private void PlayHitAnimation()
    {
        if (lizardWarriorAI != null && lizardWarriorAI.IsHealing())
        {
            animator.SetTrigger(DefendGetHit);
        }
        else
        {
            animator.SetTrigger(GetHit);
        }
    }

    public void PlayHealAnimation(bool isHealing)
    {
        animator.SetBool(IsHeal, isHealing);
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    public void OnAttackEnd() => enemyAI.OnAttackAnimationEnd();
    public void OnGetHitEnd() => enemyAI.OnGetHitAnimationEnd();
    public void OnDieEnd() => enemyHealth.OnDieAnimationEnd();
}
