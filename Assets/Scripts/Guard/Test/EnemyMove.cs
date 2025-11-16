using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void Update()
    {
        if (target == null) return;

        // 🟢 Move về trụ
        agent.SetDestination(target.position);
    }
}