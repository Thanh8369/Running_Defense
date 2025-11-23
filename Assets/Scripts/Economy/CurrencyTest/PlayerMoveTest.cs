using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển nhân vật di chuyển đến vị trí mà người chơi tap trên màn hình.
/// - Dùng cho game 3D top-down.
/// - Mobile: dùng Touch (ngón tay).
/// - Editor/PC: dùng chuột trái để test.
/// Yêu cầu:
/// - Gắn lên GameObject Player.
/// - Player phải có NavMeshAgent.
/// - Mặt đất (floor) phải có collider và nằm trong layer Ground (hoặc layer bạn chỉ định).
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class TapToMoveController : MonoBehaviour
{
    [Header("Thiết lập NavMeshAgent")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Layer mặt đất để raycast trúng")]
    [Tooltip("Chọn layer tương ứng với mặt đất (floor/ground).")]
    public LayerMask groundLayer;

    [Header("Tùy chọn xoay mặt theo hướng di chuyển")]
    public bool rotateToMoveDirection = true;
    [Tooltip("Tốc độ xoay hướng của nhân vật.")]
    public float rotateSpeed = 10f;

    private Camera mainCam;

    private void Awake()
    {
        // Lấy reference NavMeshAgent
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        // Cache main camera
        mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogWarning("[TapToMoveController] Không tìm thấy Camera.main. Hãy chắc chắn camera có tag = MainCamera.");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();   // Dùng mouse trong Editor cho tiện test
#else
        HandleTouchInput();   // Build mobile sẽ dùng touch
#endif

        HandleRotation();
    }

    /// <summary>
    /// Xử lý input mouse khi chạy trong Editor/PC.
    /// </summary>
    private void HandleMouseInput()
    {
        // Chuột trái được nhấn
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            MoveToScreenPoint(mousePos);
        }
    }

    /// <summary>
    /// Xử lý input touch khi build trên mobile.
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);

        // Chỉ xử lý khi mới chạm (Began) hoặc khi nhấc ra (Ended) tùy ý
        if (touch.phase == TouchPhase.Began)
        {
            Vector3 touchPos = touch.position;
            MoveToScreenPoint(touchPos);
        }
    }

    /// <summary>
    /// Bắn ray từ điểm trên màn hình (mouse/touch) xuống thế giới,
    /// nếu trúng Ground thì NavMeshAgent sẽ di chuyển tới đó.
    /// </summary>
    private void MoveToScreenPoint(Vector3 screenPoint)
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        // Raycast trúng layer groundLayer
        if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
        {
            Vector3 targetPoint = hit.point;
            agent.SetDestination(targetPoint);
            animator.SetFloat("Speed", 2f);

            // Nếu muốn debug vị trí tap:
            // Debug.Log("Move to: " + targetPoint);
        }
    }

    /// <summary>
    /// Xoay nhân vật theo hướng di chuyển, để nhìn mượt hơn trong game top-down.
    /// </summary>
    private void HandleRotation()
    {
        if (!rotateToMoveDirection) return;
        if (agent == null) return;
        if (agent.velocity.sqrMagnitude < 0.01f) return; // gần như đứng yên thì không xoay

        Vector3 moveDir = agent.velocity.normalized;
        moveDir.y = 0f; // chỉ xoay theo trục ngang (X,Z)

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }
}
