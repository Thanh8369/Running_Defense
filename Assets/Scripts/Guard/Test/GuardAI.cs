using UnityEngine;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour
{
    // ========================
    //        VARIABLES
    // ========================
    public TowerArea tower;
    public Transform guardPoint;
    public float moveSpeed = 3f;
    public float rotateSpeed = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private GuardAnimation guardAnim;
    private Transform target;
    private float nextAttack;

    // Mục tiêu bị khóa khi đang Attack
    private Transform lockedTarget = null;

    // Behavior tree root node
    private BTNode root;

    // ========================
    //        START
    // ========================
    void Start()
    {
        guardAnim = GetComponent<GuardAnimation>();
        BuildBehaviorTree();
    }

    // ========================
    //        UPDATE
    // ========================
    void Update()
    {
        // Nếu đang attack → KHÔNG CHO AI chạy logic DI CHUYỂN HAY ĐỔI TARGET
        if (guardAnim.isAttacking)
            return;

        // Nếu animation attack đã xong → bỏ locked target
        if (lockedTarget != null && !guardAnim.isAttacking)
            lockedTarget = null;

        UpdateTarget();
        root.Evaluate();
    }

    // ========================
    //     UPDATE TARGET
    // ========================
    void UpdateTarget()
    {
        // Nếu đang khóa mục tiêu → không đổi target
        if (lockedTarget != null)
            return;

        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        // Nếu target đang trống hoặc không còn trong queue → chọn target đầu tiên
        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    // ========================
    //          MOVE
    // ========================
    void MoveTo(Vector3 pos)
    {
        // Chặn di chuyển khi đang attack
        if (guardAnim.isAttacking)
        {
            guardAnim.SetMoving(false);
            return;
        }

        guardAnim.SetMoving(true);

        // Xoay mượt về hướng đi
        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotateSpeed * Time.deltaTime
            );
        }

        // Di chuyển
        transform.position = Vector3.MoveTowards(
            transform.position,
            pos,
            moveSpeed * Time.deltaTime
        );
    }

    // ========================
    //         ATTACK
    // ========================
    void Attack()
    {
        if (Time.time < nextAttack)
            return;

        // ------------------------------
        // KHÓA TARGET TRONG SUỐT ANIMATION
        // ------------------------------
        lockedTarget = target;

        // CHẠY ANIMATION
        guardAnim.PlayAttack();

        // Xoay về target 1 lần khi bắt đầu attack
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        nextAttack = Time.time + attackCooldown;
    }

    // ========================
    //     BUILD BEHAVIOR TREE
    // ========================
    void BuildBehaviorTree()
    {
        // ---- ATTACK ----
        var attackSequence = new BTSequence(new List<BTNode>
        {
            new BTCondition(() => target != null),
            new BTCondition(() => tower.enemyQueue.Contains(target)),
            new BTCondition(() =>
            {
                float dist = Vector3.Distance(transform.position, target.position);
                return dist <= attackRange;
            }),
            new BTAction(() =>
            {
                guardAnim.SetMoving(false);
                Attack(); // <-- KHÓA TARGET + PLAY ANIM
                return BTNode.NodeState.Success;
            })
        });

        // ---- CHASE ----
        var chaseSequence = new BTSequence(new List<BTNode>
        {
            new BTCondition(() => target != null),
            new BTCondition(() => tower.enemyQueue.Contains(target)),
            new BTAction(() =>
            {
                // Không chase khi attack
                if (guardAnim.isAttacking)
                    return BTNode.NodeState.Running;

                float dist = Vector3.Distance(transform.position, target.position);

                if (dist <= attackRange)
                    return BTNode.NodeState.Success;

                MoveTo(target.position);
                return BTNode.NodeState.Running;
            })
        });

        // ---- RETURN ----
        var returnToGuard = new BTAction(() =>
        {
            if (guardPoint == null) return BTNode.NodeState.Failure;

            if (guardAnim.isAttacking)
                return BTNode.NodeState.Running;

            float dist = Vector3.Distance(transform.position, guardPoint.position);

            if (dist < 0.5f)
            {
                guardAnim.SetMoving(false);
                return BTNode.NodeState.Success;
            }

            MoveTo(guardPoint.position);
            return BTNode.NodeState.Running;
        });

        root = new BTSelector(new List<BTNode>
        {
            attackSequence,
            chaseSequence,
            returnToGuard
        });
    }

    // ==========================================
    //   CALLED BY ANIMATION EVENT (END ATTACK)
    // ==========================================
 
}
