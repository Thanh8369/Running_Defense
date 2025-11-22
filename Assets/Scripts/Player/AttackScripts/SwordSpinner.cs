using UnityEngine;

public class SwordSpinner : MonoBehaviour
{
    //Atk speed
    public PlayerData playerData;
    public PlayerArrowDama playerDamage;
    public Transform player;
    //private float rotationsPerSecond = 1f;
    public float radius = 2f;

    void Update()
    {
        float speed = 360f / playerData.shootInterval;

        transform.RotateAround(player.position, Vector3.up, speed * Time.deltaTime);
        Vector3 offset = transform.position - player.position;
        transform.position = player.position + offset.normalized * radius;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            GetComponent<EnemyHealth>().TakeDamage(playerDamage.damage);
            //ArrowObjectPool.Instance.ReturnObject(gameObject);
            //Debug.Log("hit");
        }
    }
}
