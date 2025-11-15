using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class playercontroller : MonoBehaviour
{
    //public NavMeshAgent agent;
    //private Animator animator;

    //[Header("Dash Settings")]
    //public float doubleTapTime = 0.3f;
    //public float dashDistance = 3f;
    //public float dashSpeedMultiplier = 3f;
    //public float dashDuration = 0.2f;

    //private float lastTapTime = 0f;
    //private bool isDashing = false;
    //private float originalSpeed;

    //private Vector3 lastTapPoint;

    //void Start()
    //{
    //    originalSpeed = agent.speed;
    //    animator = GetComponent<Animator>();  // Get the Animator component
    //}

    //void Update()
    //{
    //    // For Editor testing, we use keyboard input (WASD/Arrow Keys)
    //    if (Application.isEditor)
    //    {
    //        HandleEditorMovement();
    //    }

    //    // For touch input (for mobile), we still use the original code from PlayerController
    //    if (Input.touchCount > 0)
    //    {
    //        HandleTouchInput();
    //    }
    //}

    //private void HandleEditorMovement()
    //{
    //    // Handle movement with keyboard (WASD/Arrow keys)
    //    float horizontal = Input.GetAxis("Horizontal");
    //    float vertical = Input.GetAxis("Vertical");

    //    Vector3 movement = new Vector3(horizontal, 0, vertical) * agent.speed * Time.deltaTime;

    //    if (movement.magnitude > 0)
    //    {
    //        agent.Move(movement);
    //        animator.SetFloat("Speed", 1);  // Play walking animation
    //    }
    //    else
    //    {
    //        animator.SetFloat("Speed", 0);  // Stop walking animation when idle
    //    }

    //    // Dash with Shift key (Hold for a brief moment to simulate double tap)
    //    if (Input.GetKeyDown(KeyCode.LeftShift))
    //    {
    //        if (Time.time - lastTapTime < doubleTapTime)
    //        {
    //            StartCoroutine(DashTowardPoint(transform.position + transform.forward * dashDistance));
    //        }
    //        lastTapTime = Time.time;
    //    }
    //}

    //private void HandleTouchInput()
    //{
    //    Touch touch = Input.GetTouch(0);

    //    if (touch.phase == TouchPhase.Began)
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //        if (Physics.Raycast(ray, out RaycastHit hit))
    //        {
    //            lastTapPoint = hit.point;

    //            // Check if it's a double tap
    //            if (Time.time - lastTapTime < doubleTapTime)
    //            {
    //                StartCoroutine(DashTowardPoint(lastTapPoint));
    //                animator.SetFloat("Speed", 1);
    //                animator.SetTrigger("rolling");
    //            }
    //            else
    //            {
    //                agent.SetDestination(hit.point);
    //                animator.SetFloat("Speed", 1);
    //            }
    //            lastTapTime = Time.time;
    //        }
    //    }
    //}

    //private IEnumerator DashTowardPoint(Vector3 targetPoint)
    //{
    //    if (isDashing) yield break;
    //    isDashing = true;
    //    agent.speed = originalSpeed * dashSpeedMultiplier;
    //    Vector3 direction = (targetPoint - transform.position).normalized;
    //    Vector3 dashTarget = transform.position + direction * dashDistance;
    //    agent.SetDestination(dashTarget);

    //    // Optionally, trigger dashing animation here
    //    animator.SetTrigger("rolling");

    //    yield return new WaitForSeconds(dashDuration);

    //    agent.speed = originalSpeed;
    //    isDashing = false;

    //    // After dashing, set speed back to 0 for idle
    //    animator.SetFloat("Speed", 0);
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
        animator = GetComponent<Animator>();  // Get the Animator component
        // editor = manual rotat, mobile = auto rotat
        if (Application.isEditor)
            agent.updateRotation = false;
        else
            agent.updateRotation = true;
    }

    void Update()
    {
        // For Editor testing, we use keyboard input (WASD/Arrow Keys)
        if (Application.isEditor)
        {
            HandleEditorMovement();
        }

        // For touch input (for mobile), we still use the original code from PlayerController
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }

    private void HandleEditorMovement()
    {
        // Handle movement with keyboard (WASD/Arrow keys)
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

            // Set speed for movement animation
            animator.SetFloat("Speed", 1);  // Play walking animation
        }
        else
        {
            // If not moving, set speed to 0 for idle animation
            animator.SetFloat("Speed", 0);
        }

        // Dash with Shift key (Hold for a brief moment to simulate double tap)
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // If Shift key press happens within the doubleTapTime, trigger Dash
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

                // Check if it's a double tap
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
        agent.speed = originalSpeed * dashSpeedMultiplier;

        // Dash in the direction of the target point
        Vector3 direction = (targetPoint - transform.position).normalized;
        Vector3 dashTarget = transform.position + direction * dashDistance;
        agent.SetDestination(dashTarget);

        // Trigger Dash animation
        animator.SetTrigger("rolling");

        yield return new WaitForSeconds(dashDuration);

        agent.speed = originalSpeed;
        isDashing = false;

        // After dashing, set speed back to 0 for idle animation
        animator.SetFloat("Speed", 0);
    }
}
