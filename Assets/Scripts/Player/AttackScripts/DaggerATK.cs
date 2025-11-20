using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DaggerATK : MonoBehaviour
{
    public int daggerCount;
    public float spreadAngle;
    public GameObject dagger;
    public Transform throwPoint;
    public float daggerVel;
    List<Quaternion> daggers;

    private float shootTimer = 0f;
    private void Awake()
    {
        daggers = new List<Quaternion>(daggerCount);
        for (int i = 0; i < daggerCount; i++)
        {
            daggers.Add(Quaternion.Euler(Vector3.zero));
        }
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
            ThrowDagger();
    }

    void ThrowDagger()
    {
        for (int i = 0; i < daggers.Count; i++)
        {
            //daggers[i] = Random.rotation;

            //GameObject p = Instantiate(dagger, throwPoint.position, throwPoint.rotation);
            //p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, daggers[i], spreadAngle);
            //p.GetComponent<Rigidbody>().AddForce(p.transform.forward * daggerVel);

            Quaternion spreadRot = Quaternion.AngleAxis(
            Random.Range(-spreadAngle, spreadAngle),
            throwPoint.up
        ) * Quaternion.AngleAxis(
            Random.Range(-spreadAngle, spreadAngle),
            throwPoint.right
        );
            Quaternion finalRot = throwPoint.rotation * spreadRot;
            GameObject p = Instantiate(dagger, throwPoint.position, finalRot);
            Rigidbody rb = p.GetComponent<Rigidbody>();
            rb.linearVelocity = p.transform.forward * daggerVel;
        }
    }
}
