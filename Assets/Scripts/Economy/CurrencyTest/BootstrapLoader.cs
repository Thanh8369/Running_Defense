using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [Header("Scene load đầu tiên sau Bootstrap")]
    public string nextSceneName;
    // Nếu bạn chưa có MainMenu, có thể đặt "Gameplay"

    private void Start()
    {
        // Chờ 1 frame để đảm bảo tất cả Awake của hệ thống đã chạy
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        // Đợi 1 frame – đảm bảo WalletManager Awake() xong
        yield return null;

        Debug.Log($"[BootstrapLoader] Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
