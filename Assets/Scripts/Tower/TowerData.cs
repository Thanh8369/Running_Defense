using UnityEngine;

[CreateAssetMenu(
    fileName = "TowerData",
    menuName = "Tower/Tower Data"
)]
public class TowerData : ScriptableObject
{
    [Header("Tower Stats")]
    public int maxHealth;
    public int damage;
    public float attackRange = 3f;
}