using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TowerAreaRangeRenderer : MonoBehaviour
{
    public TowerArea tower;
    public int segments = 60;
    public float heightOffset = 0.1f;

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.loop = true;
        line.widthMultiplier = 0.05f;
    }

    void Update()
    {
        DrawCircle();
    }

    void DrawCircle()
    {
        if (tower == null) return;

        float radius = tower.range;   // Phạm vi thực
        Vector3 center = tower.transform.position;

        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * (angleStep * i);

            float x = Mathf.Cos(rad) * radius;
            float z = Mathf.Sin(rad) * radius;

            Vector3 pos =
                new Vector3(x, heightOffset, z) + center;

            line.SetPosition(i, pos);
        }
    }
}