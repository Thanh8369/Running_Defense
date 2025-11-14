using UnityEngine;

/// <summary>
/// Script dùng để reset Gold về 0 mỗi khi bắt đầu 1 run mới.
/// Gắn script này vào 1 GameObject trong scene gameplay,
/// Start() sẽ tự gọi khi scene được load.
/// </summary>
public class RunGoldResetter : MonoBehaviour
{
    private void Start()
    {
        if (WalletManager.Instance == null)
        {
            Debug.LogError("[RunGoldResetter] WalletManager.Instance == null. Hãy đảm bảo có WalletManager trong scene bootstrap.");
            return;
        }

        WalletManager.Instance.ResetGoldForNewRun();
    }
}
