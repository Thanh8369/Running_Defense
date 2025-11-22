using UnityEngine;

[CreateAssetMenu(
    fileName = "TowerData",
    menuName = "Tower/Tower Data"
)]
public class TowerData : ScriptableObject
{
    [Header("Tower Stats")]
    public int maxHealth = 100;
    public int damage = 10;
    public float attackRange = 3f;
}