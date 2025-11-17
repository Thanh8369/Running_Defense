using TMPro;
using UnityEngine;

public class GoldPopupUI : MonoBehaviour
{
    public TextMeshProUGUI amountText;
    public float moveUpDistance = 50f;
    public float duration = 0.6f;

    private RectTransform _rect;
    private Vector2 _startPos;
    private float _time;

    /// <summary>
    /// Gọi ngay sau khi Instantiate và set anchoredPosition.
    /// </summary>
    public void Init(int amount)
    {
        if (_rect == null)
            _rect = GetComponent<RectTransform>();

        _startPos = _rect.anchoredPosition;

        if (amountText != null)
            amountText.text = $"+{amount}";
    }

    private void Update()
    {
        if (_rect == null) return;

        _time += Time.deltaTime;
        float t = Mathf.Clamp01(_time / duration);

        // Move lên
        _rect.anchoredPosition = _startPos + Vector2.up * moveUpDistance * t;

        // Fade
        if (amountText != null)
        {
            var c = amountText.color;
            c.a = 1f - t;
            amountText.color = c;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
