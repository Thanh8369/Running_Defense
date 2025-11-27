using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class HealthBar : MonoBehaviour
{
    public enum HealthSourceType
    {
        HealthComponent,
        PlayerRunStats,
        TowerRunStats
    }

    [Header("UI")]
    [SerializeField] private Slider _healthSlider;

    [Header("Nguồn máu")]
    [SerializeField] private HealthSourceType _sourceType = HealthSourceType.PlayerRunStats;

    [Tooltip("Dùng nếu sourceType = HealthComponent")]
    [SerializeField] private Health _health;

    [Tooltip("Dùng nếu sourceType = PlayerRunStats")]
    [SerializeField] private PlayerRunStats _playerStats;

    [Tooltip("Dùng nếu sourceType = TowerRunStats")]
    [SerializeField] private TowerRunStats _towerStats;

    [Header("Billboard / Follow Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, 0f);
    [SerializeField] private Camera _mainCamera;

    private void Awake()
    {
        // Auto gán camera
        if (_mainCamera == null)
            _mainCamera = Camera.main;

        // Auto tìm Slider nếu quên kéo
        if (_healthSlider == null)
            _healthSlider = GetComponentInChildren<Slider>();

        if (_healthSlider == null)
        {
            Debug.LogError("[HealthBar] Không tìm thấy Slider, kiểm tra child object!");
            enabled = false;
            return;
        }

        _healthSlider.minValue = 0f;
        _healthSlider.maxValue = 1f;

        // Auto tìm nguồn máu nếu quên gán
        switch (_sourceType)
        {
            case HealthSourceType.HealthComponent:
                if (_health == null)
                    _health = GetComponentInParent<Health>();
                break;

            case HealthSourceType.PlayerRunStats:
                if (_playerStats == null)
                    _playerStats = GetComponentInParent<PlayerRunStats>();
                break;

            case HealthSourceType.TowerRunStats:
                if (_towerStats == null)
                    _towerStats = GetComponentInParent<TowerRunStats>();
                break;
        }

        // Auto target = parent nếu chưa gán
        if (_target == null && transform.parent != null)
            _target = transform.parent;
    }

    private void LateUpdate()
    {
        UpdateHealthSlider();
        UpdateBillboard();
    }

    private void UpdateHealthSlider()
    {
        if (_healthSlider == null) return;

        float current = 0f;
        float max = 0f;

        switch (_sourceType)
        {
            case HealthSourceType.HealthComponent:
                if (_health == null) return;
                current = _health.CurrentHealth;
                max = _health.MaxHealth;
                break;

            case HealthSourceType.PlayerRunStats:
                if (_playerStats == null) return;
                current = _playerStats.currentHP;
                max = _playerStats.maxHP;
                break;

            case HealthSourceType.TowerRunStats:
                if (_towerStats == null) return;
                current = _towerStats.currentHP;
                max = _towerStats.maxHP;
                break;
        }

        float normalized = max > 0f ? current / max : 0f;
        _healthSlider.value = normalized;
    }

    private void UpdateBillboard()
    {
        if (_target == null || _mainCamera == null) return;

        // bay theo target + offset
        transform.position = _target.position + _offset;

        // luôn quay mặt về camera
        transform.forward = _mainCamera.transform.forward;
    }
}
