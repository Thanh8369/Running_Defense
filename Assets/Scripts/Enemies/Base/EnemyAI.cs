using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Melee, Ranged }

public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] public EnemyStats stats;

    protected Rigidbody rb;
    protected BTNode rootNode;
    protected Transform tower;
    protected Transform player;
    protected Transform currentTarget;
    protected AttackType attackType;
    protected float currentFocusTime;

    private float lastAttackTime;
    protected bool isAttacking;
    protected bool isRotate;
    protected bool isDie = false;
    protected bool isHit = false;

    public Action<bool> onMove;
    public Action<string> onAttack;

    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        Initialize(stats);
    }

    protected virtual void Initialize(EnemyStats stats)
    {
        this.stats = stats;
        SetupBT();
    }

    protected virtual void Update()
    {
        if (isDie || isHit) return;
        rootNode?.Evaluate();
        if (currentFocusTime > 0) currentFocusTime -= Time.deltaTime;
    }

    protected abstract void SetupBT();

    protected BTNode.NodeState MoveToTarget(Transform target)
    {
        if (target == null || isDie || isHit)
            return BTNode.NodeState.Failure;

        float dist = DistanceToTarget(target);
        if (isAttacking || dist <= stats.attackRange || isRotate)
        {
            onMove?.Invoke(false);
            return BTNode.NodeState.Success;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        rb.MovePosition(transform.position + dir.normalized * stats.moveSpeed * Time.deltaTime);
        onMove?.Invoke(true);
        return BTNode.NodeState.Running;
    }

    protected BTNode.NodeState RotateToTarget(Transform target)
    {
        if (target == null || isAttacking || isDie || isHit)
            return BTNode.NodeState.Failure;

        isRotate = true;
        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.001f)
        {
            isRotate = false;
            return BTNode.NodeState.Failure;
        }

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, stats.rotateSpeed * Time.deltaTime);

        if (Vector3.Angle(transform.forward, dir) <= 5f)
        {
            isRotate = false;
            return BTNode.NodeState.Success;
        }

        return BTNode.NodeState.Running;
    }

    protected BTNode.NodeState AttackTarget(Transform target)
    {
        if (target == null || !CanAttack() || isRotate || isDie || isHit)
            return BTNode.NodeState.Failure;

        isAttacking = true;
        currentTarget = target;
        string typeStr = attackType == AttackType.Melee ? "melee" : "ranged";
        onAttack?.Invoke(typeStr);
        lastAttackTime = Time.time;
        return BTNode.NodeState.Success;
    }

    public virtual void GetHit()
    {
        isHit = true;
        isAttacking = false;
        isRotate = false;
        rb.linearVelocity = Vector3.zero;
        onMove?.Invoke(false);
    }

    public virtual void StopAI()
    {
        isDie = true;
        isAttacking = false;
        isRotate = false;
        onMove?.Invoke(false);
    }

    protected float DistanceToTarget(Transform target) => target == null ? Mathf.Infinity : Vector3.Distance(transform.position, target.position);
    protected bool CanAttack() => Time.time - lastAttackTime >= stats.attackCooldown;
    public void OnAttackAnimationEnd() => isAttacking = false;
    public void OnGetHitAnimationEnd() => isHit = false;
}
