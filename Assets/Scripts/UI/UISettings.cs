using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    private const string MasterVolumeKey = "MasterVolume";
    private const string SfxVolumeKey    = "SfxVolume";
    private const string CameraFovKey    = "CameraFOV";

    [Header("Sliders")]
    [SerializeField] private Slider _bgmSlider;   // Nhạc nền
    [SerializeField] private Slider _sfxSlider;   // SFX
    [SerializeField] private Slider _fovSlider;   // FOV
    [SerializeField] private GameObject _settingsPanel; // Panel cài đặt

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;           // Nhạc nền của map hiện tại
    [SerializeField] private List<AudioSource> _sfxSources;    // Tất cả audio SFX trong map

    [Header("Camera")]
    [SerializeField] private Camera _mainCamera;  // Camera chính (hoặc camera player)

    [Header("Music Icons")]
    [SerializeField] private GameObject _musicOnIcon;   // Icon nhạc bình thường
    [SerializeField] private GameObject _musicOffIcon;  // Icon nhạc bị gạch

    private void Awake()
    {
        // --- Load giá trị đã lưu hoặc mặc định ---
        float savedBgm = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float savedSfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        float defaultFov = _mainCamera != null ? _mainCamera.fieldOfView : 60f;
        float savedFov = PlayerPrefs.GetFloat(CameraFovKey, defaultFov);

        // --- Setup sliders + listener ---
        if (_bgmSlider != null)
        {
            _bgmSlider.value = savedBgm;
            _bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = savedSfx;
            _sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }

        if (_fovSlider != null)
        {
            _fovSlider.minValue = 40f;   // tùy chỉnh theo game
            _fovSlider.maxValue = 60f;
            _fovSlider.value    = savedFov;
            _fovSlider.onValueChanged.AddListener(OnFovSliderChanged);
        }

        // --- Áp dụng giá trị khi vừa vào map ---
        ApplyBgmVolume(savedBgm);
        ApplySfxVolume(savedSfx);
        ApplyFov(savedFov);
        UpdateMusicIcons(savedBgm);
    }

    private void OnDestroy()
    {
        if (_bgmSlider != null)
            _bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);

        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);

        if (_fovSlider != null)
            _fovSlider.onValueChanged.RemoveListener(OnFovSliderChanged);
    }

    // ================== BGM / MUSIC ==================

    private void OnBgmSliderChanged(float value)
    {
        ApplyBgmVolume(value);
        SaveBgmVolume(value);
        UpdateMusicIcons(value);
    }

    private void ApplyBgmVolume(float volume)
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = volume;
        }
    }

    private void SaveBgmVolume(float volume)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void UpdateMusicIcons(float volume)
    {
        bool isMuted = volume <= 0.001f;

        if (_musicOnIcon != null)
        {
            _musicOnIcon.SetActive(!isMuted);
        }

        if (_musicOffIcon != null)
        {
            _musicOffIcon.SetActive(isMuted);
        }
    }

    // ================== SFX ==================

    private void OnSfxSliderChanged(float value)
    {
        ApplySfxVolume(value);
        SaveSfxVolume(value);
    }

    private void ApplySfxVolume(float volume)
    {
        if (_sfxSources == null)
            return;

        foreach (var source in _sfxSources)
        {
            if (source == null) continue;
            source.volume = volume;
        }
    }

    private void SaveSfxVolume(float volume)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
        PlayerPrefs.Save();
    }

    // ================== FOV ==================

    private void OnFovSliderChanged(float value)
    {
        ApplyFov(value);
        SaveFov(value);
    }

    private void ApplyFov(float fov)
    {
        if (_mainCamera != null)
        {
            _mainCamera.fieldOfView = fov;
        }
    }

    private void SaveFov(float fov)
    {
        PlayerPrefs.SetFloat(CameraFovKey, fov);
        PlayerPrefs.Save();
    }
    public void ShowSettings()
    {
        _settingsPanel.SetActive(true);
    }
    public void HideSettings()
    {
        _settingsPanel.SetActive(false);
    }
}
