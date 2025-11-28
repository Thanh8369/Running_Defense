using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Button _pauseButton;
    [SerializeField] private GameObject _pauseMenu;

    private void Awake()
    {
        if (_pauseButton == null)
        {
            _pauseButton = GetComponent<Button>();
        }

        if (_pauseButton == null)
        {
            Debug.LogError("[PauseButton] Không tìm thấy Button, kiểm tra lại Inspector!");
            enabled = false;
            return;
        }

        if (_pauseMenu == null)
        {
            Debug.LogError("[PauseButton] Không tìm thấy PauseMenu, kiểm tra lại Inspector!");
            enabled = false;
            return;
        }

        _pauseButton.onClick.AddListener(TogglePauseMenu);
    }

    private void TogglePauseMenu()
    {
        _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        Time.timeScale = _pauseMenu.activeSelf ? 0f : 1f;
    }
}
