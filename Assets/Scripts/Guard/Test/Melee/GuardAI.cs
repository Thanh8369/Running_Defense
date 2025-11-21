using UnityEngine;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour
{
    public TowerArea tower;

    public float moveSpeed = 3f;
    public float rotateSpeed = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    private GuardAnimation guardAnim;
    private Transform target;
    private float nextAttack;

    private Transform lockedTarget = null;
    private BTNode root;

    void Start()
    {
        guardAnim = GetComponent<GuardAnimation>();
        if (tower == null)
        {
            tower = FindObjectOfType<TowerArea>();
            if (tower == null)
                Debug.LogError("Không tìm thấy TowerArea trong scene!");
        }
        BuildBehaviorTree();
    }

    void Update()
    {
        // Nếu đang attack → không xử lý AI di chuyển
        if (guardAnim.isAttacking)
            return;

        // Attack xong thì mở khóa target
        if (lockedTarget != null && !guardAnim.isAttacking)
            lockedTarget = null;

        UpdateTarget();
        root.Evaluate();
    }

    // =================================================================
    // TÌM TARGET
    // =================================================================
    void UpdateTarget()
    {
        // Nếu đang khóa target khi đánh → không đổi target
        if (lockedTarget != null)
            return;

        tower.enemyQueue.RemoveAll(e => e == null);

        if (tower.enemyQueue.Count == 0)
        {
            target = null;
            return;
        }

        if (target == null || !tower.enemyQueue.Contains(target))
            target = tower.enemyQueue[0];
    }

    // =================================================================
    // DI CHUYỂN
    // =================================================================
    void MoveTo(Vector3 pos)
    {
        if (guardAnim.isAttacking)
        {
            guardAnim.SetMoving(false);
            return;
        }

        guardAnim.SetMoving(true);

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

        transform.position = Vector3.MoveTowards(
            transform.position,
            pos,
            moveSpeed * Time.deltaTime
        );
    }

    // =================================================================
    // ATTACK
    // =================================================================
    void Attack()
    {
        if (Time.time < nextAttack) return;

        lockedTarget = target; // khóa target khi animation attack

        guardAnim.PlayAttack();

        // Xoay mặt về enemy khi bắt đầu attack
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;

            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        nextAttack = Time.time + attackCooldown;
    }

    // =================================================================
    // BEHAVIOR TREE
    // =================================================================
    void BuildBehaviorTree()
    {
        // ---------------- ATTACK ----------------
        var attackSequence = new BTSequence(new List<BTNode>
        {
            new BTCondition(() => target != null),
            new BTCondition(() => tower.enemyQueue.Contains(target)),
            new BTCondition(() => Vector3.Distance(transform.position, target.position) <= attackRange),

            new BTAction(() =>
            {
                guardAnim.SetMoving(false);
                Attack();
                return BTNode.NodeState.Success;
            })
        });

        // ---------------- CHASE ----------------
        var chaseSequence = new BTSequence(new List<BTNode>
        {
            new BTCondition(() => target != null),
            new BTCondition(() => tower.enemyQueue.Contains(target)),

            new BTAction(() =>
            {
                if (guardAnim.isAttacking)
                    return BTNode.NodeState.Running;

                float dist = Vector3.Distance(transform.position, target.position);

                if (dist <= attackRange)
                    return BTNode.NodeState.Success;

                MoveTo(target.position);
                return BTNode.NodeState.Running;
            })
        });

        // ---------------- IDLE (KHÔNG CÓ TARGET) ----------------
        var idleNode = new BTAction(() =>
        {
            guardAnim.SetMoving(false);
            return BTNode.NodeState.Success;
        });

        // ---------------- ROOT ----------------
        root = new BTSelector(new List<BTNode>
        {
            attackSequence,
            chaseSequence,
            idleNode
        });
    }
}