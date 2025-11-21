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
    //Atk speed
    public PlayerData playerData;
    

    private float shootTimer = 0f;
    private void Awake()
    {
        daggers = new List<Quaternion>(daggerCount);
        for (int i = 0; i < daggerCount; i++)
        {
            daggers.Add(Quaternion.Euler(Vector3.zero));
        }
    }

     void Update()
    {
        shootTimer += Time.deltaTime;
        if ( shootTimer >= playerData.shootInterval)
        {
            ThrowDagger();
            shootTimer = 0f;
        }
    }

    void ThrowDagger()
    {
        for (int i = 0; i < daggers.Count; i++)
        {
            //1
            //daggers[i] = Random.rotation;

            //GameObject p = Instantiate(dagger, throwPoint.position, throwPoint.rotation);
            //p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, daggers[i], spreadAngle);
            //p.GetComponent<Rigidbody>().AddForce(p.transform.forward * daggerVel);

            //2
            //    Quaternion spreadRot = Quaternion.AngleAxis(
            //    Random.Range(-spreadAngle, spreadAngle),
            //    throwPoint.up
            //) * Quaternion.AngleAxis(
            //    Random.Range(-spreadAngle, spreadAngle),
            //    throwPoint.right
            //);
            //    Quaternion finalRot = throwPoint.rotation * spreadRot;
            //    GameObject p = Instantiate(dagger, throwPoint.position, finalRot);
            //    Rigidbody rb = p.GetComponent<Rigidbody>();
            //    rb.linearVelocity = p.transform.forward * daggerVel;

            //3
            Vector3 dir = throwPoint.forward;
            dir = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), throwPoint.up) * dir;     // horizontal
            dir = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), throwPoint.right) * dir;  // vertical

            // This makes blade ALWAYS point along dir without tilt
            Quaternion finalRot = Quaternion.LookRotation(dir, Vector3.up);

            GameObject p = Instantiate(dagger, throwPoint.position, finalRot);
            p.GetComponent<Rigidbody>().linearVelocity = dir.normalized * daggerVel;
        }
    }
}
