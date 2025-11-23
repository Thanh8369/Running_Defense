using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DaggerATK : MonoBehaviour
{
    public WeaponCount weaponCount;
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
        daggers = new List<Quaternion>(weaponCount.projectileCount);
        for (int i = 0; i < weaponCount.projectileCount; i++)
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
            Vector3 dir = throwPoint.forward;

            // Apply random spread
            dir = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), throwPoint.up) * dir;
            dir = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), throwPoint.right) * dir;

            Quaternion finalRot = Quaternion.LookRotation(dir, Vector3.up);

            // ---- USE POOLED OBJECT ----
            GameObject p = DaggerObjectPool.Instance.Get();

            p.transform.position = throwPoint.position;
            p.transform.rotation = finalRot;

            Rigidbody rb = p.GetComponent<Rigidbody>();
            rb.linearVelocity = dir.normalized * daggerVel;
        }
    }
}
