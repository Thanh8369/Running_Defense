using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    protected EnemyStats baseStats;
    protected Transform tower;
    protected Transform player;
    protected Rigidbody rb;
    protected BTNode rootNode;
    protected float moveSpeed;
    protected float attackDamage;
    protected float attackRange;
    protected float detectionRange;
    protected float attackCooldown;

    private float lastAttackTime;

    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(EnemyStats stats, float healthMult, float damageMult, float speedMult)
    {
        baseStats = stats;
        moveSpeed = stats.moveSpeed * speedMult;
        attackDamage = stats.attackDamage * damageMult;
        attackRange = stats.attackRange;
        detectionRange = stats.detectionRange;
        attackCooldown = stats.attackCooldown;

        SetupBT();
    }

    protected abstract void SetupBT();

    protected virtual void Update()
    {
        if (rootNode != null)
            rootNode.Evaluate();
    }

    protected BTNode.NodeState MoveTowards(Transform target)
    {
        if (target == null) return BTNode.NodeState.Failure;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);

        return BTNode.NodeState.Running;
    }

    protected BTNode.NodeState AttackTarget(Transform target)
    {
        if (target == null || !CanAttack()) return BTNode.NodeState.Failure;

        var damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
            return BTNode.NodeState.Success;
        }

        return BTNode.NodeState.Failure;
    }

    protected bool IsPlayerNearby() => player != null && Vector3.Distance(transform.position, player.position) <= detectionRange;

    protected float DistanceToTarget(Transform target) => target == null ? Mathf.Infinity : Vector3.Distance(transform.position, target.position);

    protected bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;

}
