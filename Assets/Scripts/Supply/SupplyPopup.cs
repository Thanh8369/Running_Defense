using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SupplyPopup : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform rectTransform;
    private float timer;
    private float duration;
    private Vector3 startPos;
    private float moveDistance;

    public void Init(int amount, SupplyData data)
    {
        rectTransform = GetComponent<RectTransform>();
        timer = 0f;
        duration = data.popupDuration;
        startPos = rectTransform.anchoredPosition;
        moveDistance = data.popupMoveDistance;

        if (text != null)
        {
            text.text = $"+{amount} {data.textDisplayName}";
            text.fontSize = data.textFontSize;
            text.color = data.textColor;
        }

        if (iconImage != null && data.icon != null)
        {
            iconImage.sprite = data.icon;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
        }
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // Di chuyển lên
        Vector3 newPos = startPos + Vector3.up * moveDistance * t;
        rectTransform.anchoredPosition = newPos;

        // Fade out text và icon
        if (text != null)
        {
            Color color = text.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            text.color = color;
        }
        if (iconImage != null)
        {
            Color color = iconImage.color;
            color.a = Mathf.Lerp(1f, 0f, t);
            iconImage.color = color;
        }

        if (t >= 1f)
            Destroy(gameObject);
    }
}
