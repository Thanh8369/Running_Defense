using UnityEngine;

public class BulletCanon : MonoBehaviour
{
    public float speed = 10f;
    public float height = 3f;
    public float damage = 200f;

    private Transform target;
    private Vector3 startPos;

    private float time;
    private float totalTime;

    private bool initialized = false;
    private Vector3 prevPos;

    private GameObject prefabOrigin;   // <── để biết trả về pool nào

    // Init cho PoolManager
    public void Init(Transform enemy, GameObject prefab)
    {
        prefabOrigin = prefab;          // <── lưu prefab gốc (bắt buộc cho pooling)

        target = enemy;

        startPos = transform.position;

        float distance = Vector3.Distance(startPos, enemy.position);
        totalTime = distance / speed;

        prevPos = startPos;
        time = 0f;

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        if (target == null || !target.gameObject.activeInHierarchy)
        {
            ReturnToPool();
            return;
        }

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / totalTime);

        Vector3 targetPos = target.position;

        Vector3 horizontal = Vector3.Lerp(startPos, targetPos, t);

        float curve = 4 * height * t * (1 - t);

        Vector3 nextPos = new Vector3(
            horizontal.x,
            horizontal.y + curve,
            horizontal.z
        );

        Vector3 dir = nextPos - prevPos;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        transform.position = nextPos;
        prevPos = nextPos;

        if (t >= 1f)
        {
            HitTarget();
            ReturnToPool();
        }
    }

    void HitTarget()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var c in cols)
        {
            IDamageable dmg = c.GetComponent<IDamageable>();
            if (dmg != null && c.tag != "Tower" && c.tag != "Player")
            {
                dmg.TakeDamage(damage);
                break;
            }
        }
    }

    void ReturnToPool()
    {
        initialized = false;

        if (PoolManager.Instance != null && prefabOrigin != null)
            PoolManager.Instance.Return(gameObject, prefabOrigin);
        else
            gameObject.SetActive(false);  // fallback
    }
}