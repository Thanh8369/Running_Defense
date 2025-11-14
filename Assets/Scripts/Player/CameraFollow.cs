using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -5);  // Adjust the offset to suit your game

    void LateUpdate()
    {
        // Follow the player while maintaining the specified offset
        transform.position = player.position + offset;

        // Optionally, make the camera rotate to always look at the player
        transform.LookAt(player);
    }
}
