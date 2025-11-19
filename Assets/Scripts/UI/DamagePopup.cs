using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _text;

    [Header("Animation")]
    [SerializeField] private float _moveUpSpeed = 1.5f;
    [SerializeField] private float _lifeTime = 0.6f;
    [SerializeField] private Vector3 _randomOffset = new Vector3(0.4f, 0.3f, 0f);

    [Header("Visual")]
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private float _fontSize = 3f;

    private float _timer;
    private Transform _cameraTransform;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        _cameraTransform = Camera.main != null ? Camera.main.transform : null;
    }

    public void Setup(float damage)
    {
        _timer = 0f;

        // Random lệch nhẹ để popup không trùng đúng 1 chỗ
        transform.localPosition += new Vector3(
            Random.Range(-_randomOffset.x, _randomOffset.x),
            Random.Range(0f, _randomOffset.y),
            0f
        );

        _text.text = Mathf.RoundToInt(damage).ToString();
        _text.color = _color;
        _text.fontSize = _fontSize;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        // luôn quay về phía camera
        if (_cameraTransform != null)
        {
            transform.forward = _cameraTransform.forward;
        }

        // bay lên
        transform.position += Vector3.up * (_moveUpSpeed * Time.deltaTime);

        // fade dần
        _timer += Time.deltaTime;
        float t = _timer / _lifeTime;

        Color c = _text.color;
        c.a = Mathf.Lerp(1f, 0f, t);
        _text.color = c;

        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
