using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyAnimationConfig animationConfig;

    private Animator animator;
    private EnemyAI enemy;

    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<EnemyAI>();
    }

    void OnEnable()
    {
        enemy.onMove += PlayWalkAnimation;
        enemy.onAttack += PlayAttackAnimation;
    }

    void OnDisable()
    {
        enemy.onMove -= PlayWalkAnimation;
        enemy.onAttack -= PlayAttackAnimation;
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

    public void OnAttackEnd()
    {
        enemy.OnAttackAnimationEnd();
    }
}
