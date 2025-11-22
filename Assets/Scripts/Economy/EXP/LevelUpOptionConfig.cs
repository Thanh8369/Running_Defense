using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Cấu hình cơ bản cho 1 lựa chọn Level Up.
    /// Các loại option cụ thể (Player, Tower, Passive, Skill...) sẽ kế thừa class này.
    /// </summary>
    [CreateAssetMenu(
        fileName = "LevelUpOption",
        menuName = "Son/Economy/Level Up Option/Base",
        order = 10)]
    public class LevelUpOptionConfig : ScriptableObject
    {
        [Header("Thông tin hiển thị")]
        [Tooltip("ID nội bộ, dùng cho debug / save game.")]
        public string id;

        [Tooltip("Tên hiển thị trên UI.")]
        public string displayName;

        [TextArea]
        [Tooltip("Mô tả chi tiết option.")]
        public string description;

        [Tooltip("Icon hiển thị trên nút lựa chọn.")]
        public Sprite icon;

        [Header("Giá trị cấu hình tuỳ ý")]
        [Tooltip("Giá trị 'sức mạnh' cơ bản. Tuỳ option con sử dụng như thế nào.")]
        public int powerValue = 1;

        /// <summary>
        /// Fallback: áp effect mà không truyền gì.
        /// Thường dùng cho test nhanh trong Editor.
        /// </summary>
        public virtual void ApplyEffect()
        {
            Debug.Log($"[LevelUpOptionConfig] ApplyEffect() - {id} - {displayName} (power={powerValue})");
        }

        /// <summary>
        /// Hàm chuẩn được gọi từ LevelUpPanel.
        /// Option con có thể:
        /// - Dùng playerStats để buff Player.
        /// - Bỏ qua playerStats và tự xử lý (ví dụ buff Tower).
        /// </summary>
        public virtual void ApplyEffect(PlayerRunStats playerStats)
        {
            Debug.Log($"[LevelUpOptionConfig] ApplyEffect(PlayerRunStats) - {id} - {displayName} (power={powerValue})");
        }
    }
}
