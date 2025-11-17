using UnityEngine;
using UnityEngine.AI;

public class GuardAnimation : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent agent;

    public bool isAttacking = false;
    private float attackTimer = 0f;

    public float attackDuration = 0.7f;

    void Update()
    {
        HandleMovement();
        HandleAttackTimer();
    }

    void HandleMovement()
    {
        if (isAttacking)
        {
            anim.SetBool("IsMoving", false);
            return;
        }

        bool moving = agent.velocity.sqrMagnitude > 0.1f;
        anim.SetBool("IsMoving", moving);
    }

    public void PlayAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        attackTimer = 0f;

        anim.SetTrigger("Attack");

        // Dừng di chuyển NGAY, KHÔNG tắt NavMeshAgent
        agent.ResetPath();
        agent.velocity = Vector3.zero;   // chặn trượt lúc xoay
    }

    void HandleAttackTimer()
    {
        if (!isAttacking) return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            isAttacking = false;

            // KHÔNG cần bật lại agent
            // Vì agent chưa bao giờ bị tắt!
        }
    }
}