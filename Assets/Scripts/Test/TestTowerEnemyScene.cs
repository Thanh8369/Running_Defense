using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTowerEnemyScene : MonoBehaviour, IDamageable
{
    public float health = 1000f;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Tower Destroyed!");
        }
    }
}
