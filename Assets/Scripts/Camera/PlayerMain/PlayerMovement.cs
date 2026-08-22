using UnityEngine;
using PinePie.SimpleJoystick;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public JoystickController joystick; // kéo object chứa JoystickController vào đây
    public Transform cameraTransform; // kéo Main Camera vào đây

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; // tốc độ xoay nhân vật hướng theo di chuyển

    [Header("Gravity")]
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (joystick == null) return;

        // Lấy input từ joystick (Vector2: x = trái/phải, y = trên/dưới)
        Vector2 input = joystick.InputDirection;

        Vector3 moveDirection;

        if (cameraTransform != null)
        {
            // Lấy hướng forward/right của camera, bỏ qua trục Y (chỉ quan tâm mặt phẳng X-Z)
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Hướng di chuyển = input.y theo hướng camera nhìn tới + input.x theo hướng ngang camera
            moveDirection = camForward * input.y + camRight * input.x;
        }
        else
        {
            // Fallback: nếu chưa gán camera, dùng trục thế giới như cũ
            moveDirection = new Vector3(input.x, 0f, input.y);
        }

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // Di chuyển nhân vật
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Xoay nhân vật hướng về phía đang di chuyển
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Áp dụng trọng lực để nhân vật không bị lơ lửng / xuyên sàn
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}