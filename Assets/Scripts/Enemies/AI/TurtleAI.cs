using System.Collections.Generic;
using UnityEngine;

public class TurtleAI : MeleeEnemyAI
{
    [Header("Defense Stance Settings")]
    [SerializeField] private float defenseHPThreshold = 0.5f;
    [SerializeField] private float defenseDamageReduction = 0.5f;

    private bool isDefending = false;
    private EnemyHealth enemyHealth;
    private EnemyAnimation enemyAnimation;

    protected override void Start()
    {
        base.Start();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAnimation = GetComponent<EnemyAnimation>();
    }

    protected override List<BTNode> GetAdditionalBTNodes()
    {
        return new List<BTNode> {
            new BTSequence(new List<BTNode> {
                new BTCondition(() => ShouldDefend()),
                new BTAction(() => EnterDefenseStance())
            })
        };
    }

    private bool ShouldDefend()
    {
        return !isDefending && enemyHealth != null && enemyHealth.GetHealthPercent() <= defenseHPThreshold;
    }

    private BTNode.NodeState EnterDefenseStance()
    {
        if (!isDefending)
        {
            isDefending = true;
            enemyAnimation?.PlayHealAnimation(true);
        }
        return BTNode.NodeState.Success;
    }

    public float GetDamageReductionMultiplier()
    {
        // Nếu đang trong defense stance thì giảm damage, giữ nguyên đến khi chết
        return isDefending ? (1f - defenseDamageReduction) : 1f;
    }
}
