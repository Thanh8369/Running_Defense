using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartGameController : MonoBehaviour
{
    [Header("Loading UI")]
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TMP_Text _loadingText;

    [Header("Config")]
    [SerializeField] private string _sceneName;      // tên scene map 1
    [SerializeField] private float _loadingDuration = 5f;     // 5s

    private bool _isLoading;

    private void Awake()
    {
        if (_loadingPanel != null)
        {
            _loadingPanel.SetActive(false);   // đảm bảo tắt lúc vào game
        }
    }

    // Gán hàm này vào OnClick của nút Start
    public void OnStartButtonPressed()
    {
        if (_isLoading) return;

        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
    _isLoading = true;

    if (_loadingPanel != null)
        _loadingPanel.SetActive(true);

    if (_loadingSlider != null)
        _loadingSlider.value = 0f;

    if (_loadingText != null)
        _loadingText.text = "Loading... 0%";

    float startTime = Time.unscaledTime;
    float endTime = startTime + _loadingDuration;

    while (Time.unscaledTime < endTime)
    {
        float progress = Mathf.InverseLerp(startTime, endTime, Time.unscaledTime);

        if (_loadingSlider != null)
            _loadingSlider.value = progress;

        if (_loadingText != null)
            _loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";

        yield return null;
    }
    if (_loadingSlider != null)
        _loadingSlider.value = 1f;

    if (_loadingText != null)
        _loadingText.text = "Loading... 100%";

    yield return new WaitForSecondsRealtime(0.1f);

    SceneManager.LoadScene(_sceneName);
    }
}
