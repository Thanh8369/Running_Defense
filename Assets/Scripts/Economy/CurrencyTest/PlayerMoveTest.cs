using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
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
    [HideInInspector] public bool isRolling = false;

    [Header("Roll Move")]
    public float rollDistance = 4f;
    public float rollDuration = 0.25f;   // thời gian LERP để đi hết quãng đường

    private float rollTimer;
    private Vector3 rollStartPos;
    private Vector3 rollEndPos;

    private float lastTapTime = 0f;
    private bool arrived = true;

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
    }

    // Khi script bị Disable (player chết / disable control) thì clear path luôn
    private void OnDisable()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        isRolling = false;
        arrived = true;

        if (playerCollider != null)
            playerCollider.enabled = true;
    }

    private void Update()
    {
        // Không cho điều khiển khi player chết / đang revive
        if (PlayerLifeController.Instance != null &&
            !PlayerLifeController.Instance.CanAct)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            if (animator != null)
                animator.SetFloat("Speed", 0f);

            isRolling = false;
            arrived = true;

            if (playerCollider != null)
                playerCollider.enabled = true;

            return;
        }

        // Đang roll: chỉ LERP vị trí, không xử lý input
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            float t = Mathf.Clamp01(rollTimer / rollDuration);
            transform.position = Vector3.Lerp(rollStartPos, rollEndPos, t);
            return;
        }

#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        HandleRotation();
        CheckArrived();
    }

    // =====================================================
    // CHECK ARRIVE
    // =====================================================
    private void CheckArrived()
    {
        if (arrived || agent == null) return;

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f))
        {
            arrived = true;

            agent.isStopped = true;
            agent.updateRotation = false;
            agent.velocity = Vector3.zero;

            if (animator != null)
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
        if (mainCam == null || agent == null) return;

        // Raycast lấy vị trí tap để biết hướng lướt
        Ray ray = mainCam.ScreenPointToRay(screenPoint);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            return;

        rollDirection = hit.point - transform.position;
        rollDirection.y = 0f;
        if (rollDirection.sqrMagnitude < 0.001f)
            return;

        rollDirection.Normalize();

        // set start & end
        rollStartPos = transform.position;
        rollEndPos = rollStartPos + rollDirection * rollDistance;
        rollTimer = 0f;

        isRolling = true;
        arrived = true; // trong lúc roll coi như đã tới

        // NavMesh trong lúc roll
        agent.isStopped = true;
        agent.updateRotation = false;
        agent.ResetPath();

        // Xoay mặt theo hướng roll
        transform.rotation = Quaternion.LookRotation(rollDirection);

        // Tắt collider để khỏi va chạm
        if (playerCollider != null)
            playerCollider.enabled = false;

        // Bất tử trong lúc roll
        if (PlayerLifeController.Instance != null)
            PlayerLifeController.Instance.SetInvulnerable(true);

        // Trigger animation
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetTrigger("Roll");
        }

        // Đợi animation xong rồi mở lại control
        StartCoroutine(WaitForRollEnd());
    }

    private System.Collections.IEnumerator WaitForRollEnd()
    {
        // Đợi 1 frame cho chắc là vào state Roll
        yield return null;

        // Chờ animation kết thúc (tùy clip của bạn)
        yield return new WaitForSeconds(1.8f);

        isRolling = false;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            agent.velocity = Vector3.zero;
        }

        if (playerCollider != null)
            playerCollider.enabled = true;

        // Hết roll thì hết bất tử
        if (PlayerLifeController.Instance != null)
            PlayerLifeController.Instance.SetInvulnerable(false);
    }

    // =====================================================
    // MOVE TO TAP
    // =====================================================
    private void MoveToScreenPoint(Vector3 screenPoint)
    {
        if (mainCam == null || agent == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            arrived = false;

            agent.isStopped = false;
            agent.updateRotation = true;

            agent.ResetPath();
            agent.SetDestination(hit.point);

            if (animator != null)
                animator.SetFloat("Speed", 2f);
        }
    }

    // =====================================================
    // ROTATION WHILE MOVING
    // =====================================================
    private void HandleRotation()
    {
        if (!rotateToMoveDirection || isRolling || agent == null) return;
        if (agent.velocity.sqrMagnitude < 0.01f) return;

        Vector3 dir = agent.velocity.normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
    }
}
