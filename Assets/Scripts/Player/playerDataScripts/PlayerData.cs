using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float shootInterval;
    public float detectionRange;
}
