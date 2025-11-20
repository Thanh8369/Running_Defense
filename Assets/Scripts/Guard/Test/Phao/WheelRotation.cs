using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public Transform wheelLeft;
    public Transform wheelRight;

    public float wheelRotateSpeed = 360f; // độ xoay mỗi giây

    private bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            // Xoay bánh theo trục local X
            wheelLeft.Rotate(Vector3.right * wheelRotateSpeed * Time.deltaTime, Space.Self);
            wheelRight.Rotate(Vector3.right * wheelRotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }
}