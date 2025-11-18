using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;

    public bool isAttacking = false;

    private RigidbodyConstraints originalConstraints;

    void Start()
    {
        originalConstraints = rb.constraints;
    }

    void Update()
    {
        // không dùng attackTimer nữa!
    }

    public void SetMoving(bool moving)
    {
        if (isAttacking)
        {
            anim.SetBool("IsMoving", false);
            return;
        }

        anim.SetBool("IsMoving", moving);
    }

    public void PlayAttack()
    {
        if (isAttacking) return;

        isAttacking = true;

        anim.SetTrigger("Attack");
        anim.SetBool("IsMoving", false);

        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // 🔥 Animation Event tại cuối Animation Attack
    public void OnAttackEnd()
    {
        isAttacking = false;

        // mở lại chuyển động
        rb.constraints = originalConstraints;
    }
}