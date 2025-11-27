using UnityEngine;

/// <summary>
/// Script debug để test PlayerLifeController:
/// - Bấm phím P trên bàn phím để cho player chết.
/// - Có hàm public KillPlayer() để gán vào UI Button OnClick.
/// </summary>
public class PlayerLifeDebugTester : MonoBehaviour
{
    [Header("Tham chiếu đến PlayerLifeController")]
    public PlayerLifeController playerLife;

    [Header("Lượng damage giả để đảm bảo chết")]
    public float fakeDamage = 99999f;

    private void Awake()
    {
        // Nếu quên gán trong Inspector thì tự tìm
        if (playerLife == null)
        {
            playerLife = FindAnyObjectByType<PlayerLifeController>();

            if (playerLife == null)
            {
                Debug.LogWarning("[PlayerLifeDebugTester] Không tìm thấy PlayerLifeController trong scene.");
            }
        }
    }

    private void Update()
    {
        // Nhấn phím P để kill player (chỉ để test trong Editor / PC)
        if (Input.GetKeyDown(KeyCode.U))
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Gọi hàm này từ UI Button (OnClick) để cho player chết.
    /// </summary>
    public void KillPlayer()
    {
        if (playerLife == null)
        {
            Debug.LogWarning("[PlayerLifeDebugTester] Chưa gán PlayerLifeController.");
            return;
        }

        // Gây damage đủ lớn để chắc chắn chết
        playerLife.ApplyDamage(fakeDamage);
        Debug.Log("[PlayerLifeDebugTester] KillPlayer() → Gửi damage để test chết.");
    }
}
