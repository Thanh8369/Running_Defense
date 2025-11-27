using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerAreaRangeRenderer : MonoBehaviour
{
    public TowerArea towerArea;
    private TowerRunStats stats;

    private LineRenderer line;
    public int segments = 40;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.widthMultiplier = 0.05f;

        // --- FIX QUAN TRỌNG ---
        if (towerArea == null)
            towerArea = GetComponentInParent<TowerArea>();

        if (towerArea == null)
        {
            Debug.LogError("Không tìm thấy TowerArea!");
            return;
        }

        // --- GÁN TowerRunStats ---
        stats = towerArea.GetComponent<TowerRunStats>();

        if (stats == null)
        {
            Debug.LogError("TowerArea không có TowerRunStats!");
            return;
        }

        DrawCircle(stats.attackRange);
    }

    void Update()
    {
        if (stats != null)
            DrawCircle(stats.attackRange); // realtime update khi nâng cấp
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

            // NÂNG VÒNG LÊN 1 CHÚT CHO KHÔNG CHÌM DƯỚI ĐẤT
            line.SetPosition(i, new Vector3(x, 2f, z));

            angle += step;
        }
    }
}