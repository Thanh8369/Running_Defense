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

        // Freeze toàn bộ khi tấn công
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Gọi từ Animation Event cuối attack
    public void OnAttackEnd()
    {
        isAttacking = false;

        // Mở khóa chuyển động lại
        rb.constraints = originalConstraints;
    }
}