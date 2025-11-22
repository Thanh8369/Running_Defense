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

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsHealing = Animator.StringToHash("IsHealing");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int GetHit = Animator.StringToHash("GetHit");
    private static readonly int DefendGetHit = Animator.StringToHash("DefendGetHit");
    private static readonly int Die = Animator.StringToHash("Die");

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

        animator.Rebind();
        animator.Update(0f);
        animator.SetBool(IsMoving, false);
        animator.SetBool(IsHealing, false);
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

    private void PlayWalkAnimation(bool isMoving, float moveSpeed)
    {
        animator.SetBool(IsMoving, isMoving);
        animator.SetFloat(MoveSpeed, moveSpeed);
    }

    private void PlayAttackAnimation()
    {
        if (animationConfig == null) return;

        EnemyAnimationData anim = animationConfig.GetRandomAttack(animationConfig.attackAnimations);

        if (anim != null)
        {
            animator.SetTrigger(anim.triggerName);

            cyclopsAI?.OnAttackAnimationSet(anim.triggerName);
            evilMageAI?.OnAttackAnimationSet(anim.triggerName);
            orcAI?.OnAttackAnimationSet(anim.triggerName);
            lizardWarriorAI?.OnAttackAnimationSet(anim.triggerName);
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
        animator.SetBool(IsHealing, isHealing);
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    public void OnAttackEnd() => enemyAI.OnAttackAnimationEnd();
    public void OnGetHitEnd() => enemyAI.OnGetHitAnimationEnd();
    public void OnDieEnd() => enemyHealth.OnDieAnimationEnd();
}
