using UnityEngine;

public class testAutoarrow : MonoBehaviour
{
    public PlayerData playerData;
     
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float shootTimer = 0f;

    void Update()
    {
        shootTimer += Time.deltaTime;

        GameObject nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null && shootTimer >= playerData.shootInterval)
        {
            Shoot(nearestEnemy.transform);
            shootTimer = 0f;
        }
    }

    GameObject GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, currentPos);

            if (dist < minDistance && dist <= playerData.detectionRange)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    void Shoot(Transform target)
    {
        GameObject proj = ArrowObjectPool.Instance.GetObject();
        //GameObject proj = Instantiate(projectilePrefab);

        proj.transform.position = firePoint.position;
        proj.transform.rotation = Quaternion.identity;

        Vector3 dir = (target.position - firePoint.position).normalized;
       
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * 20f;

        proj.transform.forward = dir;
    }
}
