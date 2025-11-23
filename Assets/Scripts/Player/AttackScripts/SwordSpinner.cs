using UnityEngine;

public class SwordSpinner : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;
    public PlayerRunStats playerRunStats;

    [Header("Config")]
    public float radius = 2f;

    private void Awake()
    {
        // Nếu chưa gán trong Inspector thì tự tìm
        if (playerRunStats == null)
        {
            playerRunStats = FindAnyObjectByType<PlayerRunStats>();
            if (playerRunStats != null && player == null)
            {
                player = playerRunStats.transform;
            }
        }
    }

    void Update()
    {
        if (player == null || playerRunStats == null) return;

        // attackSpeed = số đòn / giây => số vòng quay / giây
        float rotationsPerSecond = playerRunStats.baseSwordAttackSpeed;
        float speed = 360f * rotationsPerSecond; // độ/giây

        // Quay quanh player
        transform.RotateAround(player.position, Vector3.up, speed * Time.deltaTime);

        // Giữ khoảng cách cố định (radius)
        Vector3 offset = transform.position - player.position;
        if (offset.sqrMagnitude > 0.001f)
        {
            transform.position = player.position + offset.normalized * radius;
        }
        else
        {
            // Nếu sword trùng đúng vị trí player (hiếm khi) thì đặt nó ra trước mặt
            transform.position = player.position + player.forward * radius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerRunStats == null) return;

        if (other.TryGetComponent<EnemyHealth>(out var damageable))
        {
            damageable.TakeDamage(playerRunStats.TotalSwordDamage);
        }
    }
}
