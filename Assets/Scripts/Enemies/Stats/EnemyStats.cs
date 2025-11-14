using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Game/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float attackCooldown = 1f;
    
    [Header("Visual")]
    public GameObject prefab;
    
    [Header("Rewards")]
    public int goldReward = 10;
    public int expReward = 5;
}
