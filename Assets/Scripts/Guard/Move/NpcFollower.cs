using UnityEngine;

public class NpcFollower : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Formation")]
    public int slotIndex;         // vị trí thứ mấy trong đội (0 đến 5)
    public int totalSlots = 6;    // tổng số NPC hiện có
    public float formationRadius = 2f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotateSpeed = 10f;
    public float stopDistance = 0.1f; // để tránh rung lắc khi đã tới home

    private Vector3 targetPos;

    void Update()
    {
        // 1. Tính vị trí home theo formation (vòng tròn quanh player)
        targetPos = GetFormationPosition();

        // 2. Di chuyển tới vị trí đó nếu còn cách xa
        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist > stopDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, moveSpeed * Time.deltaTime
            );

            // xoay mặt theo hướng di chuyển cho mượt (nếu sprite/model có hướng)
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }
    }

    Vector3 GetFormationPosition()
    {
        float angleStep = 360f / totalSlots;
        float angle = angleStep * slotIndex * Mathf.Deg2Rad;

        // Dùng X-Z cho mặt phẳng ngang (Y giữ nguyên độ cao)
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * formationRadius;
        return player.position + offset;
    }
}