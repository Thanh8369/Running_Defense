using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Runtime Stats")]
    public float health;
    public float maxHealth;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float detectionRange;
    public float attackCooldown;
    
    public EnemyStats baseStats;
    protected Transform tower;
    protected Transform player;
    protected Rigidbody rb;
    protected BTNode rootNode;
    
    private float lastAttackTime;
    public System.Action<EnemyBase> onDeath;
    
    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Initialize(baseStats, 1f, 1f, 1f);
    }

    public virtual void Initialize(EnemyStats stats, float healthMult, float damageMult, float speedMult)
    {
        baseStats = stats;
        maxHealth = stats.maxHealth * healthMult;
        health = maxHealth;
        moveSpeed = stats.moveSpeed * speedMult;
        attackDamage = stats.attackDamage * damageMult;
        attackRange = stats.attackRange;
        detectionRange = stats.detectionRange;
        attackCooldown = stats.attackCooldown;
        
        SetupBehaviourTree();
    }
    
    protected abstract void SetupBehaviourTree();
    
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }
        
        if (rootNode != null)
            rootNode.Evaluate();
    }
    
    protected bool IsPlayerNearby() => player != null && Vector3.Distance(transform.position, player.position) <= detectionRange;
    
    protected float DistanceToTarget(Transform target) => target == null ? Mathf.Infinity : Vector3.Distance(transform.position, target.position);
    
    protected bool CanAttack() => Time.time - lastAttackTime >= attackCooldown;
    
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
    
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
    }
    
    protected virtual void Die()
    {
        if (baseStats != null)
        {
            // Give rewards
            Debug.Log($"Enemy defeated! +{baseStats.goldReward} gold, +{baseStats.expReward} exp");
        }
        
        onDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
