using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TapToMoveController : MonoBehaviour
{
    [Header("NavMeshAgent Settings")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Ground Layer")]
    public LayerMask groundLayer;

    [Header("Rotate To Move Direction")]
    public bool rotateToMoveDirection = true;
    public float rotateSpeed = 10f;

    [Header("Double Tap Roll Settings")]
    public float doubleTapTime = 0.25f;
    public bool isRolling = false;
    public float extraRollPush = 0f; // thêm lực khi roll nếu animation yếu

    private float lastTapTime = 0f;
    private bool arrived = false;

    private Camera mainCam;
    private Vector3 rollDirection;
    private Collider playerCollider;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogWarning("[TapToMove] MainCamera not found");

        playerCollider = GetComponent<Collider>();
        if (playerCollider == null)
            Debug.LogWarning("[TapToMove] Collider not found");
    }

    private void Update()
    {
        if (isRolling) return; // đang roll thì không nhận input di chuyển

#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        HandleRotation();
        CheckArrived();
    }

    private void FixedUpdate()
    {
        if (isRolling && extraRollPush > 0f)
        {
            // extra push nếu animation roll không có rootmotion mạnh
            transform.position += rollDirection * extraRollPush * Time.deltaTime;
        }
    }

    // =====================================================
    // CHECK ARRIVE
    // =====================================================
    private void CheckArrived()
    {
        if (arrived) return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        {
            arrived = true;

            agent.isStopped = true;
            agent.updateRotation = false;
            agent.velocity = Vector3.zero;

            animator.SetFloat("Speed", 0f);
        }
    }

    // =====================================================
    // PC INPUT
    // =====================================================
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckDoubleTap())
            {
                TriggerRoll(Input.mousePosition);
                return;
            }

            MoveToScreenPoint(Input.mousePosition);
        }
    }

    // =====================================================
    // MOBILE INPUT
    // =====================================================
    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (CheckDoubleTap())
            {
                TriggerRoll(touch.position);
                return;
            }

            MoveToScreenPoint(touch.position);
        }
    }

    // =====================================================
    // DOUBLE TAP
    // =====================================================
    private bool CheckDoubleTap()
    {
        float now = Time.time;

        if (now - lastTapTime <= doubleTapTime)
        {
            lastTapTime = 0f;
            return true;
        }

        lastTapTime = now;
        return false;
    }

    // =====================================================
    // ROLL
    // =====================================================
    private void TriggerRoll(Vector3 screenPoint)
    {
        if (mainCam == null) return;

        // Raycast để lấy vị trí tap
        Ray ray = mainCam.ScreenPointToRay(screenPoint);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            return;

        // Tính hướng roll
        rollDirection = (hit.point - transform.position);
        rollDirection.y = 0;
        rollDirection.Normalize();

        // Xoay về hướng roll
        if (rollDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rollDirection);

        // Bắt đầu roll
        isRolling = true;
        arrived = false;

        // Tắt agent
        agent.isStopped = true;
        agent.updateRotation = false;

        // Tắt collider để không va chạm khi roll
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Trigger animation
        animator.SetTrigger("Roll");

        StartCoroutine(WaitForRollEnd());
    }

    private System.Collections.IEnumerator WaitForRollEnd()
    {
        // Đợi animation vào state Roll
        yield return new WaitForSeconds(0.05f);

        // Chờ animation kết thúc
        yield return new WaitForSeconds(1.8f); // thời gian roll animation

        // Kết thúc roll
        isRolling = false;
        agent.isStopped = false;
        agent.updateRotation = true;

        // Bật lại collider
        if (playerCollider != null)
            playerCollider.enabled = true;
    }

    // =====================================================
    // MOVE TO TAP
    // =====================================================
    private void MoveToScreenPoint(Vector3 screenPoint)
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            arrived = false;

            agent.isStopped = false;
            agent.updateRotation = true;

            agent.SetDestination(hit.point);
            animator.SetFloat("Speed", 2f);
        }
    }

    // =====================================================
    // ROTATION WHILE MOVING
    // =====================================================
    private void HandleRotation()
    {
        if (!rotateToMoveDirection || isRolling) return;
        if (agent.velocity.sqrMagnitude < 0.01f) return;

        Vector3 dir = agent.velocity.normalized;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
    }
}
