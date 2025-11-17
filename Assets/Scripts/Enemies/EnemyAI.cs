using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Melee, Ranged }

public abstract class EnemyAI : MonoBehaviour
{
    protected EnemyStats baseStats;
    protected Rigidbody rb;
    protected BTNode rootNode;
    protected Transform tower;
    protected Transform player;
    protected Transform currentTarget;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float detectionRange;
    protected AttackType attackType;

    protected float lastAttackTime;
    protected float currentFocusTime;

    protected bool isMoving;
    protected bool isAttacking;
    protected bool isRotatingToTarget;

    // EVENTS
    public Action<bool> onMove;
    public Action<string> onAttack;

    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(EnemyStats stats)
    {
        baseStats = stats;
        moveSpeed = stats.moveSpeed;
        rotateSpeed = stats.rotateSpeed;
        attackDamage = stats.attackDamage;
        attackCooldown = stats.attackCooldown;
        attackRange = stats.attackRange;
        detectionRange = stats.detectionRange;

        SetupBT();
    }

    protected virtual void Update()
    {
        rootNode?.Evaluate();

        if (currentFocusTime > 0)
            currentFocusTime -= Time.deltaTime;

        if (!isMoving)
            onMove?.Invoke(false);

        isMoving = false;
    }

    protected abstract void SetupBT();

    // MOVE TO TARGET
    protected BTNode.NodeState MoveToTarget(Transform target)
    {
        if (target == null) return BTNode.NodeState.Failure;

        float dist = DistanceToTarget(target);

        if (isAttacking || dist <= attackRange || isRotatingToTarget)
        {
            onMove?.Invoke(false);
            isMoving = false;
            return BTNode.NodeState.Success;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        rb.MovePosition(transform.position + dir.normalized * moveSpeed * Time.deltaTime);

        isMoving = true;
        onMove?.Invoke(true);

        return BTNode.NodeState.Running;
    }

    // ROTATE TO TARGET
    protected BTNode.NodeState RotateToTarget(Transform target)
    {
        if (target == null || isAttacking)
            return BTNode.NodeState.Failure;

        isRotatingToTarget = true;

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f)
        {
            isRotatingToTarget = false;
            return BTNode.NodeState.Failure;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );

        if (Vector3.Angle(transform.forward, dir) <= 5f)
        {
            isRotatingToTarget = false;
            return BTNode.NodeState.Success;
        }

        return BTNode.NodeState.Running;
    }

    // ATTACK TARGET - Animation only
    protected BTNode.NodeState AttackTarget(Transform target)
    {
        if (target == null || !CanAttack() || isRotatingToTarget)
            return BTNode.NodeState.Failure;

        isAttacking = true;
        currentTarget = target;

        string typeStr = attackType == AttackType.Melee ? "melee" : "ranged";
        onAttack?.Invoke(typeStr);

        lastAttackTime = Time.time;
        return BTNode.NodeState.Success;
    }

    // HELPERS
    protected float DistanceToTarget(Transform target) => target == null ? Mathf.Infinity : Vector3.Distance(transform.position, target.position);
    protected bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;
    public void OnAttackAnimationEnd() => isAttacking = false;
}
