using UnityEngine;

public class FlyingDagger : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(rb.linearVelocity);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, target, Time.fixedDeltaTime * 10f));
        }
    }
}
