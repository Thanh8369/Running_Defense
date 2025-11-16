using UnityEngine;

public class GoldPopupSpawner : MonoBehaviour
{
    public static GoldPopupSpawner Instance { get; private set; }

    [Header("References")]
    [Tooltip("Canvas HUD (Screen Space Overlay hoặc Screen Space Camera)")]
    public Canvas mainCanvas;

    [Tooltip("Prefab popup UI (có GoldPopupUI + Text)")]
    public GameObject goldPopupPrefab;

    [Tooltip("Camera thế giới (nếu để trống sẽ dùng Camera.main)")]
    public Camera worldCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnGoldPopup(Vector3 worldPos, int amount)
    {
        if (goldPopupPrefab == null)
        {
            Debug.LogWarning("[GoldPopupSpawner] goldPopupPrefab == null");
            return;
        }

        // Tự tìm Canvas nếu chưa gán
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                Debug.LogWarning("[GoldPopupSpawner] Không tìm thấy Canvas trong scene hiện tại.");
                return;
            }
        }

        // Tự tìm Camera nếu chưa gán
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
            {
                Debug.LogWarning("[GoldPopupSpawner] Không tìm thấy Camera.main.");
                return;
            }
        }

        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();

        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        Vector2 uiLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCanvas.worldCamera,
            out uiLocalPos
        );

        GameObject popupObj = Instantiate(goldPopupPrefab, mainCanvas.transform, false);
        RectTransform popupRect = popupObj.GetComponent<RectTransform>();
        popupRect.anchoredPosition = uiLocalPos;

        GoldPopupUI popupUI = popupObj.GetComponent<GoldPopupUI>();
        if (popupUI != null)
        {
            popupUI.Init(amount);
        }

        Debug.Log($"[GoldPopupSpawner] Spawn popup tại {uiLocalPos}, amount={amount}");
    }
}
