using UnityEngine;

public class FlyingDagger : MonoBehaviour
{
    public Rigidbody rb;
    public float rotateSpeed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            var targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime));
        }
    }
}
