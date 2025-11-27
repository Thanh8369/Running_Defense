using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { AttackSpeed, MoveSpeed }

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
    public float currentAttackSpeed { get; private set; }

    protected Rigidbody rb;
    protected BTNode rootNode;
    protected Transform tower;
    protected Transform player;
    protected Transform currentTarget;
    protected Transform nearestTroop;
    protected float lastAttackTime;

    private float troopScanInterval = 0.5f;
    private float lastTroopScanTime;

    protected bool isAttacking;
    protected bool isRotate;
    protected bool isDie = false;
    protected bool isHit = false;
    private List<Buff> activeBuffs = new List<Buff>();

    public Action<bool, float> OnMove;
    public Action OnAttack;

    protected virtual void Start()
    {
        tower = GameObject.FindGameObjectWithTag("Tower")?.transform;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody>();

        currentMoveSpeed = stats.moveSpeed;
        currentAttackSpeed = stats.attackSpeed;

        //if (tower != null)
        //{
        //    TowerRunStats hp = player.GetComponent<TowerRunStats>();
        //    hp.OnDeath += OnPlayerDeath;
        //    hp.OnRevive += OnPlayerRevive;

        //    if (hp.currentHP <= 0)
        //        player = null;
        //}

        if (player != null)
        {
            PlayerLifeController hp = player.GetComponent<PlayerLifeController>();
            hp.OnDeath += OnPlayerDeath;
            hp.OnRevive += OnPlayerRevive;

            //if (hp.currentHP <= 0)
            //    player = null;
        }

        SetupBT();
    }

    private void OnPlayerDeath()
    {
        if (player != null)
        {
            player = null;
            if (currentTarget != null && currentTarget.CompareTag("Player"))
                currentTarget = null;
        }
    }

    private void OnPlayerRevive()
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
        currentTarget = null;
        nearestTroop = null;

        currentMoveSpeed = stats.moveSpeed;
        currentAttackSpeed = stats.attackSpeed;

        activeBuffs.Clear();

        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    protected virtual void Update()
    {
        if (isDie || isHit) return;

        if (Time.time - lastTroopScanTime >= troopScanInterval)
        {
            FindNearestTroop();
            lastTroopScanTime = Time.time;
        }

        rootNode?.Evaluate();

        UpdateBuffs();
    }

    protected abstract void SetupBT();

    // =======================================================
    // TARGET
    // =======================================================
    protected void FindNearestTroop()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag("Troop");

        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject troop in troops)
        {
            if (troop == null) continue;

            // Kiểm tra troop còn alive
            HealthGuard guard = troop.GetComponent<HealthGuard>();
            if (guard != null && guard.isDead) continue;

            float dist = Vector3.Distance(transform.position, troop.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = troop.transform;
            }
        }

        nearestTroop = closest;

        // Nếu currentTarget đã chết thì đổi target
        if (currentTarget != null)
        {
            HealthGuard guard = currentTarget.GetComponent<HealthGuard>();
            if (guard != null && guard.isDead)
                currentTarget = nearestTroop != null ? nearestTroop : tower;
        }
    }

    // =======================================================
    // MOVEMENT
    // =======================================================
    protected BTNode.NodeState MoveToTarget(Transform target)
    {
        if (target == null || isDie || isHit)
            return BTNode.NodeState.Failure;

        float dist = DistanceToTarget(target);

        if (isAttacking || dist <= stats.attackRange || isRotate)
        {
            OnMove?.Invoke(false, 0);
            return BTNode.NodeState.Success;
        }

        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        rb.MovePosition(transform.position + dir.normalized * currentMoveSpeed * Time.deltaTime);
        OnMove?.Invoke(true, Mathf.Clamp01(currentMoveSpeed / stats.maxSpeed));

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

        lastAttackTime = Time.time;
        OnAttack?.Invoke();

        return BTNode.NodeState.Success;
    }

    // =======================================================
    // HIT / DIE
    // =======================================================
    public virtual void GetHit()
    {
        isHit = true;
        isAttacking = false;
        isRotate = false;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        OnMove?.Invoke(false, 0);
    }

    public virtual void StopAI()
    {
        isDie = true;
        isAttacking = false;
        isRotate = false;

        OnMove?.Invoke(false, 0);
    }

    // =======================================================
    // BUFF FIXED
    // =======================================================
    private void UpdateBuffs()
    {
        float attackSpeedMultiplier = 1f;
        float moveSpeedMultiplier = 1f;

        // Lặp từ cuối để remove buff hết hạn
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            var buff = activeBuffs[i];

            if (Time.time >= buff.endTime)
            {
                activeBuffs.RemoveAt(i);
                continue;
            }

            // Không stack: lấy buff cuối cùng của loại đó
            if (buff.type == BuffType.AttackSpeed)
                attackSpeedMultiplier = buff.multiplier;
            else if (buff.type == BuffType.MoveSpeed)
                moveSpeedMultiplier = buff.multiplier;
        }

        currentAttackSpeed = stats.attackSpeed * attackSpeedMultiplier;
        currentMoveSpeed = stats.moveSpeed * moveSpeedMultiplier;
    }

    public void ApplyBuff(BuffType type, float multiplier, float duration)
    {
        activeBuffs.Add(new Buff
        {
            type = type,
            multiplier = multiplier,
            duration = duration,
            endTime = Time.time + duration
        });
    }

    // =======================================================
    // HELPERS
    // =======================================================
    protected float DistanceToTarget(Transform target)
    {
        return target == null ? Mathf.Infinity :
            Vector3.Distance(transform.position, target.position);
    }
    protected bool CanAttack() => Time.time - lastAttackTime >= (1f / stats.attackSpeed);
    public float GetAttackSpeed() => currentAttackSpeed;
    public void OnAttackAnimationEnd() => isAttacking = false;
    public void OnGetHitAnimationEnd() => isHit = false;
}
