using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { Speed }

[Serializable]
public class Buff
{
    public BuffType type;
    public float multiplier = 1f;
    public float duration = 0f;
    [NonSerialized] public float endTime;
}

public abstract class EnemyAI : MonoBehaviour
{
    public EnemyStats stats;
    public float currentMoveSpeed { get; private set; }

    protected Rigidbody rb;
    protected BTNode rootNode;
    protected Transform tower;
    protected Transform player;
    protected Transform currentTarget;
    protected Transform nearestTroop;
    protected float currentFocusTime;
    protected float lastAttackTime;

    private float troopScanInterval = 0.5f;
    private float lastTroopScanTime;

    protected bool isAttacking;
    protected bool isRotate;
    protected bool isDie = false;
    protected bool isHit = false;
    private List<Buff> activeBuffs = new List<Buff>();

    public Action<bool, float> onMove;
    public Action onAttack;

    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();

        currentMoveSpeed = stats.moveSpeed;

        player.GetComponent<Health>().OnDeath += HandlePlayerDeath;
        player.GetComponent<Health>().OnRevive += HandlePlayerRevive;

        if (player.GetComponent<Health>().CurrentHealth <= 0)
        {
            player = null;
        }

        SetupBT();
    }

    private void HandlePlayerDeath()
    {
        if (player != null)
        {
            player = null;
            if (currentTarget != null && currentTarget.CompareTag("Player"))
                currentTarget = null;
        }
    }

    private void HandlePlayerRevive()
    {
        Transform newPlayer = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (newPlayer != null)
        {
            player = newPlayer;
            currentTarget = player;
        }
    }

    protected virtual void OnEnable()
    {
        isDie = false;
        isHit = false;
        isAttacking = false;
        isRotate = false;

        lastAttackTime = -999f;
        lastTroopScanTime = -999f;
        currentFocusTime = 0f;
        currentTarget = null;
        nearestTroop = null;

        currentMoveSpeed = stats.moveSpeed;

        activeBuffs.Clear();

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    protected virtual void Update()
    {
        if (isDie || isHit) return;

        // Scan troops định kỳ
        if (Time.time - lastTroopScanTime >= troopScanInterval)
        {
            FindNearestTroop();
            lastTroopScanTime = Time.time;
        }

        rootNode?.Evaluate();

        if (currentFocusTime > 0) currentFocusTime -= Time.deltaTime;

        UpdateBuffs();
    }

    protected abstract void SetupBT();

    // ============ Target Finding ============
    protected void FindNearestTroop()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

        if (troops.Length == 0)
        {
            nearestTroop = null;
            return;
        }

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject troop in troops)
        {
            if (troop == null) continue;

            float distance = Vector3.Distance(transform.position, troop.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = troop.transform;
            }
        }

        nearestTroop = closest;
    }

    // ============ Movement ============
    protected BTNode.NodeState MoveToTarget(Transform target)
    {
        if (target == null || isDie || isHit)
            return BTNode.NodeState.Failure;

        float dist = DistanceToTarget(target);
        if (isAttacking || dist <= stats.attackRange || isRotate)
        {
            onMove?.Invoke(false, 0);
            return BTNode.NodeState.Success;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        rb.MovePosition(transform.position + dir.normalized * currentMoveSpeed * Time.deltaTime);
        onMove?.Invoke(true, Mathf.Clamp01(currentMoveSpeed / stats.maxSpeed));
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
        onAttack?.Invoke();
        lastAttackTime = Time.time;
        return BTNode.NodeState.Success;
    }

    public virtual void GetHit()
    {
        isHit = true;
        isAttacking = false;
        isRotate = false;
        if (rb != null) rb.linearVelocity = Vector3.zero;
        onMove?.Invoke(false, 0);
    }

    public virtual void StopAI()
    {
        isDie = true;
        isAttacking = false;
        isRotate = false;
        onMove?.Invoke(false, 0);
    }

    private void UpdateBuffs()
    {
        float multiplierSpeed = 1f;
        Buff lastSpeed = null;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];
            if (Time.time >= buff.endTime)
            {
                activeBuffs.RemoveAt(i);
                continue;
            }

            if (buff.type == BuffType.Speed && lastSpeed == null)
                lastSpeed = buff;
        }

        if (lastSpeed != null)
            multiplierSpeed = lastSpeed.multiplier;

        float targetSpeed = Mathf.Min(stats.moveSpeed * multiplierSpeed, stats.maxSpeed);
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetSpeed, 10f * Time.deltaTime);
    }

    public void ApplyBuff(BuffType type, float multiplier, float duration)
    {
        Buff buff = new Buff()
        {
            type = type,
            multiplier = multiplier,
            duration = duration,
            endTime = Time.time + duration
        };

        activeBuffs.Add(buff);
    }

    protected float DistanceToTarget(Transform target) => target == null ? Mathf.Infinity : Vector3.Distance(transform.position, target.position);
    protected bool CanAttack() => Time.time - lastAttackTime >= stats.attackCooldown;
    public void OnAttackAnimationEnd() => isAttacking = false;
    public void OnGetHitAnimationEnd() => isHit = false;
}
