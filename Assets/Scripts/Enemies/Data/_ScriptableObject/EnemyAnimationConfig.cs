using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyAnimationData
{
    public string triggerName;    
    [Range(1f, 99f)] public float percent = 20f;           
    [HideInInspector] public float lastUsedTime = -999f;
}

[CreateAssetMenu(fileName = "AnimConfig", menuName = "Enemy/Animation Config")]
public class EnemyAnimationConfig : ScriptableObject
{
    public List<EnemyAnimationData> meleeAttacks = new List<EnemyAnimationData>();
    public List<EnemyAnimationData> rangedAttacks = new List<EnemyAnimationData>();
    public List<EnemyAnimationData> specialAttacks = new List<EnemyAnimationData>();

    public EnemyAnimationData GetRandomAttack(List<EnemyAnimationData> attackList)
    {
        if (attackList == null || attackList.Count == 0)
            return null;

        var selected = WeightedRandom(attackList);

        if (selected != null)
            selected.lastUsedTime = Time.time;

        return selected;
    }

    private EnemyAnimationData WeightedRandom(List<EnemyAnimationData> attacksAnimations)
    {
        if (attacksAnimations.Count == 0) return null;
        if (attacksAnimations.Count == 1) return attacksAnimations[0];

        float totalPercent = attacksAnimations.Sum(a => a.percent);
        float randomValue = UnityEngine.Random.Range(0f, totalPercent);

        float current = 0f;
        foreach (var a in attacksAnimations)
        {
            current += a.percent;
            if (randomValue <= current)
                return a;
        }

        return attacksAnimations[0];
    }
}
