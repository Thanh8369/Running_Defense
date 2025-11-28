using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerAreaRangeRenderer : MonoBehaviour
{
    public TowerArea towerArea;
    private TowerRunStats stats;

    private LineRenderer line;
    private SphereCollider rangeCollider; // <-- thêm collider
    public int segments = 40;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.05f;

        // Lấy TowerArea
        if (towerArea == null)
            towerArea = GetComponentInParent<TowerArea>();

        if (towerArea == null)
        {
            Debug.LogError("Không tìm thấy TowerArea!");
            return;
        }

        // Lấy stats
        stats = towerArea.GetComponent<TowerRunStats>();
        if (stats == null)
        {
            Debug.LogError("TowerArea không có TowerRunStats!");
            return;
        }

        // Lấy SphereCollider
        rangeCollider = GetComponent<SphereCollider>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<SphereCollider>();
            rangeCollider.isTrigger = true; // collider này thường dùng trigger
        }

        UpdateRange(stats.attackRange);
    }

    void Update()
    {
        if (stats != null)
            UpdateRange(stats.attackRange); // realtime update khi nâng cấp
    }

    void UpdateRange(float radius)
    {
        DrawCircle(radius);

        if (rangeCollider != null)
            rangeCollider.radius = radius; // <-- thay đổi radius collider
    }

    void DrawCircle(float radius)
    {
        if (line == null) return;

        line.positionCount = segments + 1;

        float angle = 0f;
        float step = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;

            line.SetPosition(i, new Vector3(x, 2f, z));
            angle += step;
        }
    }
}