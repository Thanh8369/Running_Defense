using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Enemy/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    
    [Header("Rewards")]
    public int minGold = 5;
    public int maxGold = 15;
    public int expAmount = 10;
    
    [Header("Visual")]
    public GameObject prefab;
    public GameObject projectilePrefab;
}