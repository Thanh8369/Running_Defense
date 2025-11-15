using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Enemies/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float attackCooldown = 1f;
    
    [Header("Visual")]
    public GameObject prefab;
}
