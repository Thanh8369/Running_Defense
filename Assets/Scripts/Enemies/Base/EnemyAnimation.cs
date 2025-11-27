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
    private WerewolfAI werewolfAI;
    private EnemyHealth enemyHealth;

    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int IsHealing = Animator.StringToHash("IsHealing");
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int GetHit = Animator.StringToHash("GetHit");
    private static readonly int DefendGetHit = Animator.StringToHash("DefendGetHit");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int AttackSpeedParam = Animator.StringToHash("AttackSpeed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();
        cyclopsAI = GetComponent<CyclopsAI>();
        evilMageAI = GetComponent<EvilMageAI>();
        orcAI = GetComponent<OrcAI>();
        lizardWarriorAI = GetComponent<LizardWarriorAI>();
        werewolfAI = GetComponent<WerewolfAI>();
    }

    private void OnEnable()
    {
        if (enemyAI != null)
        {
            enemyAI.OnMove += PlayWalkAnimation;
            enemyAI.OnAttack += PlayAttackAnimation;
        }

        if (enemyHealth != null)
        {
            enemyHealth.OnHit += PlayHitAnimation;
            enemyHealth.OnDeath += PlayDeathAnimation;
        }

        // Reset animator để tránh trigger lỗi khi re-enable
        animator.Rebind();
        animator.Update(0f);
    }

    private void OnDisable()
    {
        if (enemyAI != null)
        {
            enemyAI.OnMove -= PlayWalkAnimation;
            enemyAI.OnAttack -= PlayAttackAnimation;
        }

        if (enemyHealth != null)
        {
            enemyHealth.OnHit -= PlayHitAnimation;
            enemyHealth.OnDeath -= PlayDeathAnimation;
        }
    }

    // ================= WALK =================
    private void PlayWalkAnimation(bool isMoving, float moveSpeed)
    {
        animator.SetBool(IsMoving, isMoving);
        animator.SetFloat(MoveSpeed, moveSpeed);
    }

    // ================= ATTACK =================
    private void PlayAttackAnimation()
    {
        // Nếu là Werewolf, dùng trigger cụ thể dựa vào target
        if (werewolfAI != null)
        {
            string triggerName = werewolfAI.GetCurrentAttackTrigger();
            animator.SetTrigger(triggerName);
            float attackSpeed = enemyAI.GetAttackSpeed();
            animator.SetFloat(AttackSpeedParam, attackSpeed);
            return;
        }

        // Các enemy khác dùng random animation
        if (animationConfig == null) return;

        EnemyAnimationData anim = animationConfig.GetRandomAttack(animationConfig.attackAnimations);
        if (anim == null) return;

        // Trigger animation
        animator.SetTrigger(anim.triggerName);

        // Set tốc độ attack animation dựa vào attackCooldown
        float attackSpeed2 = enemyAI.GetAttackSpeed();
        animator.SetFloat(AttackSpeedParam, attackSpeed2);

        // Gửi trigger cho AI đặc thù
        cyclopsAI?.OnAttackAnimationSet(anim.triggerName);
        evilMageAI?.OnAttackAnimationSet(anim.triggerName);
        orcAI?.OnAttackAnimationSet(anim.triggerName);
        lizardWarriorAI?.OnAttackAnimationSet(anim.triggerName);
    }

    // ================= HIT =================
    private void PlayHitAnimation()
    {
        if (lizardWarriorAI != null && lizardWarriorAI.IsHealing())
            animator.SetTrigger(DefendGetHit);
        else
            animator.SetTrigger(GetHit);
    }

    // ================= HEAL =================
    public void PlayHealAnimation(bool isHealing)
    {
        animator.SetBool(IsHealing, isHealing);
    }

    // ================= DIE =================
    private void PlayDeathAnimation()
    {
        animator.SetTrigger(Die);
    }

    // ================= ANIMATION EVENTS =================
    public void OnAttackEnd() => enemyAI.OnAttackAnimationEnd();
    public void OnGetHitEnd() => enemyAI.OnGetHitAnimationEnd();
    public void OnDieEnd() => enemyHealth.OnDieAnimationEnd();
}
