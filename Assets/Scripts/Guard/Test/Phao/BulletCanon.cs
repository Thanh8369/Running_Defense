using UnityEngine;

public class BulletCanon : MonoBehaviour
{
    public float speed = 10f;      // tốc độ bay ngang
    public float height = 3f;      // độ cao parabola
    public float damage = 200f;    // damage gây ra

    private Vector3 targetPos;
    private Vector3 startPos;

    private float time;
    private float totalTime;

    private bool initialized = false;
    private Vector3 prevPos;

    public void Init(Vector3 target)
    {
        startPos = transform.position;
        targetPos = target;

        float distance = Vector3.Distance(startPos, targetPos);
        totalTime = distance / speed;

        prevPos = startPos;

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / totalTime);

        // Đi ngang
        Vector3 horizontal = Vector3.Lerp(startPos, targetPos, t);

        // Cong hình parabola
        float curve = 4 * height * t * (1 - t);

        Vector3 nextPos = new Vector3(
            horizontal.x,
            horizontal.y + curve,
            horizontal.z
        );

        // Xoay đạn theo hướng bay
        Vector3 dir = nextPos - prevPos;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // Cập nhật vị trí
        transform.position = nextPos;
        prevPos = nextPos;

        // Đến nơi
        if (t >= 1f)
        {
            HitTarget();
            Destroy(gameObject);
        }
    }

    void HitTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var c in cols)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }
    }
}
