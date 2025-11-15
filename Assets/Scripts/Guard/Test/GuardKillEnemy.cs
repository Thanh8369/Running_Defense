using UnityEngine;

public class GuardKillEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Guard touched enemy → Enemy destroyed!");
            Destroy(other.gameObject);
        }
    }
}