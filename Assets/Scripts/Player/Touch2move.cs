using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class Touch2move : MonoBehaviour
{
    //public NavMeshAgent agent;  
    //void Start()
    //{

    //}  
    //void Update()
    //{
    //    if(Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);

    //        if(touch.phase == TouchPhase.Began)
    //        {
    //            Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //            RaycastHit hit;

    //            if(Physics.Raycast(ray, out hit))
    //            {
    //                agent.destination = hit.point;
    //            }
    //        }
    //    }
    //}

    public NavMeshAgent agent;
    private Animator animator;

    [Header("Dash Settings")]
    public float doubleTapTime = 0.3f;   
    public float dashDistance = 3f;      
    public float dashSpeedMultiplier = 3f;
    public float dashDuration = 0.2f;

    private float lastTapTime = 0f;
    private bool isDashing = false;
    private float originalSpeed;

    private Vector3 lastTapPoint;
    void Start()
    {
        originalSpeed = agent.speed;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    lastTapPoint = hit.point;                    
                    if (Time.time - lastTapTime < doubleTapTime)
                    {
                        StartCoroutine(DashTowardPoint(lastTapPoint));
                        animator.SetFloat("Speed", 1);
                    }
                    else
                    {                      
                        agent.SetDestination(hit.point);
                        animator.SetFloat("Speed", 1);
                    }
                    lastTapTime = Time.time;
                }
            }
        }
    }

    private IEnumerator DashTowardPoint(Vector3 targetPoint)
    {
        if (isDashing) yield break;
        isDashing = true;      
        agent.speed = originalSpeed * dashSpeedMultiplier;      
        Vector3 direction = (targetPoint - transform.position).normalized;        
        Vector3 dashTarget = transform.position + direction * dashDistance;
        agent.SetDestination(dashTarget);
        //animation trigger
        animator.SetTrigger("rolling");

        yield return new WaitForSeconds(dashDuration);
        agent.speed = originalSpeed;
        isDashing = false;
        //reset to idle
        animator.SetFloat("Speed", 0);
    }
}
