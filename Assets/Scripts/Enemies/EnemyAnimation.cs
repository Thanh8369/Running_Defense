using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAnimationConfig animationConfig;

    private Animator animator;
    private EnemyAI enemyAI;
    private EnemyHealth enemyHealth;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int GetHit = Animator.StringToHash("GetHit");
    private static readonly int Die = Animator.StringToHash("Die");

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void OnEnable()
    {
        enemyAI.onMove += PlayWalkAnimation;
        enemyAI.onAttack += PlayAttackAnimation;
        
        if (enemyHealth != null)
        {
            enemyHealth.onHit += PlayGetHitAnimation;
            enemyHealth.onDie += PlayDeathAnimation;
        }
    }

    void OnDisable()
    {
        enemyAI.onMove -= PlayWalkAnimation;
        enemyAI.onAttack -= PlayAttackAnimation;
        
        if (enemyHealth != null)
        {
            enemyHealth.onHit -= PlayGetHitAnimation;
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

        EnemyAnimationData anim =
            atkType == "ranged"
            ? animationConfig.GetRandomAttack(animationConfig.rangedAttacks)
            : animationConfig.GetRandomAttack(animationConfig.meleeAttacks);

        if (anim != null)
            animator.SetTrigger(anim.triggerName);
    }

    public void PlayGetHitAnimation()
    {
        animator.SetTrigger(GetHit);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    public void OnAttackEnd()
    {
        enemyAI.OnAttackAnimationEnd();
    }

    public void OnGetHitEnd()
    {
        enemyAI.OnGetHitAnimationEnd();
    }

    public void OnDieEnd()
    {
        enemyHealth.OnDieAnimationEnd();
    }
}
