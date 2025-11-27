using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    public GameObject prefab;
    public float customDamage = -1f; // -1 = use Enemy damage, >0 = use custom
}

[Serializable]
public class AttackVariant
{
    public string triggerName;
    public float damageAmount = 15f;
}

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Enemy/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;
    [HideInInspector] public float maxSpeed = 5f;
    [HideInInspector] public float rotateSpeed = 360f;
    public float attackDamage = 10f;
    [Min(1)] public float attackSpeed = 1f;
    public float attackRange = 2f;
    public float detectionRange = 8f;

    [Header("Rewards")]
    public int minGold = 5;
    public int maxGold = 15;
    public int expAmount = 10;

    [Header("Visual")]
    public GameObject prefab;

    [Header("Attack Variants")]
    public List<AttackVariant> attackVariants = new List<AttackVariant>();

    [Header("Projectile Settings")]
    public List<ProjectileData> projectilePrefabs = new List<ProjectileData>();

    private void OnValidate()
    {
        maxSpeed = moveSpeed * 2f;
    }
}
