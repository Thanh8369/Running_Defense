using UnityEngine;

namespace Son.Economy
{
    public class CameraFollowClamp : MonoBehaviour
    {
        [Header("Player để follow")]
        public Transform target;          // Drag Player vào đây

        [Header("Offset so với Player")]
        public Vector3 offset = new Vector3(0f, 10f, -10f);  // Top-down hoặc 3rd person

        [Header("Mượt hay không")]
        [Range(0f, 10f)]
        public float followSpeed = 5f;    // 0 = không mượt, 5–10 = mượt

        [Header("Giới hạn vùng camera (World)")]
        public float minX = -20f;
        public float maxX = 20f;
        public float minZ = -20f;
        public float maxZ = 20f;

        private void LateUpdate()
        {
            if (target == null) return;

            // 1. Vị trí camera muốn tới = vị trí player + offset
            Vector3 desiredPos = target.position + offset;

            // 2. Clamp X/Z để camera không vượt khỏi map
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.z = Mathf.Clamp(desiredPos.z, minZ, maxZ);
            // Giữ nguyên Y theo offset (thường là cao hơn player)
            // desiredPos.y = desiredPos.y; // khỏi cần ghi

            // 3. Lerp cho mượt (nếu không thích mượt thì gán thẳng transform.position = desiredPos)
            transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

            // (Optional) luôn nhìn xuống player
            // transform.LookAt(target);
        }
    }
}