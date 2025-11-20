using UnityEngine;

public class GoldPopupSpawner : MonoBehaviour
{
    public static GoldPopupSpawner Instance { get; private set; }

    [Header("Canvas để spawn popup (Sorting thấp hơn UISkill)")]
    public Canvas popupCanvas;

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

        // nếu chưa gán thì tìm tên Canvas_GoldPopup
        if (popupCanvas == null)
        {
            popupCanvas = GameObject.Find("Canvas_PopupGold")?.GetComponent<Canvas>();

            if (popupCanvas == null)
            {
                Debug.LogError("[GoldPopupSpawner] Bạn CHƯA gán popupCanvas!");
                return;
            }
        }

        if (worldCamera == null)
            worldCamera = Camera.main;

        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();

        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        Vector2 uiLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            popupCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : popupCanvas.worldCamera,
            out uiLocalPos
        );

        GameObject popupObj = Instantiate(goldPopupPrefab, popupCanvas.transform, false);

        RectTransform popupRect = popupObj.GetComponent<RectTransform>();
        popupRect.anchoredPosition = uiLocalPos;

        GoldPopupUI popupUI = popupObj.GetComponent<GoldPopupUI>();
        if (popupUI != null)
        {
            popupUI.Init(amount);
        }

        // Debug
        // Debug.Log($"[GoldPopupSpawner] Spawn popup tại {uiLocalPos}, amount={amount}");
    }
}
