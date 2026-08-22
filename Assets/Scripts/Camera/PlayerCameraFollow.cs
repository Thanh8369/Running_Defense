using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // kéo Player vào đây trong Inspector

    [Header("Offset (khoảng cách camera so với player)")]
    public Vector3 cameraOffset = new Vector3(0f, 12f, -8f); // chỉnh theo góc nghiêng bạn muốn

    [Header("Smooth Follow")]
    public float smoothSpeed = 5f; // càng lớn càng bám sát, càng nhỏ càng mượt/trễ

    [Header("Tùy chọn")]
    public bool lookAtTarget = true; // camera có tự xoay nhìn về player không

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí mong muốn = vị trí player + offset
        Vector3 desiredPosition = target.position + cameraOffset;

        // Di chuyển mượt tới vị trí đó thay vì snap ngay lập tức
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Nếu muốn camera luôn nhìn về phía player
        if (lookAtTarget)
        {
            transform.LookAt(target);
        }
    }
}