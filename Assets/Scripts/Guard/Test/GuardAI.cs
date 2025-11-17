using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GuardAI : MonoBehaviour
{
public TowerArea tower;
public NavMeshAgent agent;
public Transform guardPoint;
public float attackRange = 2f;
public float attackCooldown = 1f;
private GuardAnimation guardAnim;


    private Transform target;
private float nextAttack;
private BTNode root;

void Start()
{
    BuildBehaviorTree();
    guardAnim = GetComponent<GuardAnimation>();
}

void Update()
{
    UpdateTarget();
    root.Evaluate();
}

// --- UPDATE TARGET LOGIC ---
void UpdateTarget()
{
    tower.enemyQueue.RemoveAll(e => e == null);

    if (tower.enemyQueue.Count == 0)
    {
        target = null;
        return;
    }

    if (target == null || !tower.enemyQueue.Contains(target))
    {
        target = tower.enemyQueue[0];
    }
}

// --- GUARD RETURN ---
void Attack()
{
        if (Time.time >= nextAttack)
        {
            Debug.Log("Guard Attack!");
            guardAnim.PlayAttack();   // << GỌI ANIMATION Ở ĐÂY
            nextAttack = Time.time + attackCooldown;
        }
    }

// ==============================
// ==    BUILD BEHAVIOR TREE   ==
// ==============================
void BuildBehaviorTree()
{
    // ---- ATTACK SEQUENCE ----
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
                Attack();
                return BTNode.NodeState.Success;
            })
        });

    // ---- CHASE SEQUENCE ----
    var chaseSequence = new BTSequence(new List<BTNode>
        {
            new BTCondition(() => target != null),
            new BTCondition(() => tower.enemyQueue.Contains(target)),
            new BTAction(() =>
            {
                if (target == null)
                    return BTNode.NodeState.Failure;

                float dist = Vector3.Distance(transform.position, target.position);

                if (dist < 1f)
                {
                    agent.SetDestination(transform.position);
                    return BTNode.NodeState.Success;
                }

                agent.SetDestination(target.position);
                return BTNode.NodeState.Running;
            })
        });

    // ---- RETURN TO GUARD POINT ----
    var returnToGuard = new BTAction(() =>
    {
        if (guardPoint == null)
            return BTNode.NodeState.Failure;

        float dist = Vector3.Distance(transform.position, guardPoint.position);

        if (dist < 1f)
        {
            agent.SetDestination(transform.position);
            return BTNode.NodeState.Success;
        }

        agent.SetDestination(guardPoint.position);
        return BTNode.NodeState.Running;
    });

    // ---- ROOT SELECTOR ----
    root = new BTSelector(new List<BTNode>
        {
            attackSequence,
            chaseSequence,
            returnToGuard
        });
}
}
