using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Health _health;
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Camera _mainCamera;

    private void Awake()
    {
        // Auto gán camera
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        // Auto tìm Health nếu quên kéo
        if (_health == null)
        {
            _health = GetComponentInParent<Health>();
        }

        if (_health == null)
        {
            Debug.LogError("[HealthBar] Không tìm thấy Health, kiểm tra lại Inspector!");
            enabled = false;
            return;
        }

        // Auto tìm Slider nếu quên kéo
        if (_healthSlider == null)
        {
            _healthSlider = GetComponentInChildren<Slider>();
        }

        if (_healthSlider == null)
        {
            Debug.LogError("[HealthBar] Không tìm thấy Slider, kiểm tra child object!");
            enabled = false;
            return;
        }

        // Đảm bảo slider chuẩn 0–1
        _healthSlider.minValue = 0f;
        _healthSlider.maxValue = 1f;

        _health.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(_health.CurrentHealth, _health.MaxHealth);

        Debug.Log("[HealthBar] Awake xong, đã subscribe event.");
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        if (_healthSlider == null) return;

        float normalized = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        _healthSlider.value = normalized;

        Debug.Log($"[HealthBar] Health Updated: {currentHealth}/{maxHealth} (slider = {normalized})");
    }

    private void LateUpdate()
    {
        if (_target == null || _mainCamera == null) return;

        transform.position = _target.position + _offset;
        transform.forward = _mainCamera.transform.forward;
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnHealthChanged -= OnHealthChanged;
        }
    }

    // TEST bằng SPACE
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[HealthBar] SPACE pressed, gọi TakeDamage(10f)");
            _health.TakeDamage(10f);
        }
    }
}
