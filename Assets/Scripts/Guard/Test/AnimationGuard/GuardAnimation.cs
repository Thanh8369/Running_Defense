using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;

    public bool isAttacking = false;
    public bool isDead = false;

    private RigidbodyConstraints originalConstraints;

    void Start()
    {
        originalConstraints = rb.constraints;
    }

    public void SetMoving(bool moving)
    {
        if (isAttacking || isDead)
        {
            anim.SetBool("IsMoving", false);
            return;
        }

        anim.SetBool("IsMoving", moving);
    }

    public void PlayAttack()
    {
        if (isAttacking || isDead) return;

        isAttacking = true;
        anim.SetTrigger("Attack");
        anim.SetBool("IsMoving", false);

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void OnAttackEnd()
    {
        if (isDead) return;

        isAttacking = false;
        rb.constraints = originalConstraints;
    }

    // ------------------------------------
    // 🟥 GỌI KHI CHẾT
    // ------------------------------------
    public void PlayDie()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Die");
        anim.SetBool("IsMoving", false);

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}