using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class playercontroller : MonoBehaviour
{
    //dash eff
    public TrailRenderer dashTrail;


    public NavMeshAgent agent;
    private Animator animator;

    
    private float doubleTapTime = 0.3f;
    private float dashDistance = 3f;
    private float dashSpeedMultiplier = 2f;
    private float dashDuration = 0.1f;

    private float lastTapTime = 0f;
    private bool isDashing = false;
    private float originalSpeed;

    private Vector3 lastTapPoint;
    
    void Start()
    {
        //tr.emitting = false;
        originalSpeed = agent.speed;
        animator = GetComponent<Animator>();

        if (dashTrail != null)
            dashTrail.emitting = false;
        // editor = manual rotat, mobile = auto rotat
        if (Application.isEditor)
            agent.updateRotation = false;
        else
            agent.updateRotation = true;
    }

    void Update()
    {
        // For Editor testing
        if (Application.isEditor)
        {
            HandleEditorMovement();
        }

        // For touch input (for mobile) PlayerController
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }

    private void HandleEditorMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // If horizontal or vertical input is detected, calculate movement direction
        Vector3 movement = new Vector3(horizontal, 0, vertical) * agent.speed * Time.deltaTime;

        if (movement.magnitude > 0)
        {
            // Move the NavMeshAgent
            agent.Move(movement);

            ////////
            RotatePlayer(movement);
            animator.SetFloat("Speed", 1);  
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {           
            if (Time.time - lastTapTime < doubleTapTime)
            {
                StartCoroutine(DashTowardPoint(transform.position + transform.forward * dashDistance));
            }
            lastTapTime = Time.time;
        }
    }

    private void HandleTouchInput()
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
                    animator.SetTrigger("rolling");
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

    private void RotatePlayer(Vector3 movement)
    {
        // Rotate player to face the direction of movement
        if (movement.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }

    private IEnumerator DashTowardPoint(Vector3 targetPoint)
    {
        if (isDashing) yield break;
        isDashing = true;

        // ENABLE TRAIL
        if (dashTrail != null)
            dashTrail.enabled = true;

        agent.speed = originalSpeed * dashSpeedMultiplier;

        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 dashTarget = transform.position + direction * dashDistance;

        agent.SetDestination(dashTarget);

        animator.SetTrigger("rolling");

        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);

        agent.speed = originalSpeed;
        isDashing = false;

        // DISABLE TRAIL
        if (dashTrail != null)
            dashTrail.enabled = false;

        animator.SetFloat("Speed", 0);
    }
}
