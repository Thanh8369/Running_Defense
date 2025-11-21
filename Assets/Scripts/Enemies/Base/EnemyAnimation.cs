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
    private EnemyHealth enemyHealth;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int GetHit = Animator.StringToHash("GetHit");
    private static readonly int DefendGetHit = Animator.StringToHash("DefendGetHit");
    private static readonly int Die = Animator.StringToHash("Die");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        cyclopsAI = GetComponent<CyclopsAI>();
        evilMageAI = GetComponent<EvilMageAI>();
        enemyHealth = GetComponent<EnemyHealth>();
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
        }
    }

    private void PlayHitAnimation()
    {
        animator.SetTrigger(GetHit);
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    public void OnAttackEnd() => enemyAI.OnAttackAnimationEnd();
    public void OnGetHitEnd() => enemyAI.OnGetHitAnimationEnd();
    public void OnDieEnd() => enemyHealth.OnDieAnimationEnd();
}
