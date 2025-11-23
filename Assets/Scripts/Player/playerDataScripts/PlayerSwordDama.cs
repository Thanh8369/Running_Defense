using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/Sword Damage", fileName = "SwordDamageData")]
public class PlayerSwordDama : ScriptableObject
{
    [Tooltip("Base damage của kiếm.")]
    public float damage = 10f;
    public float SwordAttackSpeed;
}
