using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Nâng cấp khi Level Up (miễn phí).
    /// </summary>
    [CreateAssetMenu(fileName = "LevelUpOption", menuName = "Son/Economy/Level Up Option/Base", order = 10)]
    public class LevelUpOptionConfig : ScriptableObject
    {
        [Header("Thông tin hiển thị")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Cấu hình tuỳ ý")]
        public int powerValue = 1;  // ví dụ: +1 damage, +5% attack speed...

        /// <summary>
        /// Thực thi hiệu ứng nâng cấp (cũ – không có tham số).
        /// Dùng trong trường hợp bạn tự Find PlayerRunStats bên trong.
        /// </summary>
        public virtual void ApplyEffect()
        {
            Debug.Log($"[LevelUpOption] ApplyEffect (no stats param): {id} - {displayName} (power={powerValue})");
        }

        /// <summary>
        /// Bản chuẩn: truyền PlayerRunStats để cộng stat.
        /// </summary>
        public virtual void ApplyEffect(PlayerRunStats stats)
        {
            Debug.Log($"[LevelUpOption] ApplyEffect(PlayerRunStats): {id} - {displayName} (power={powerValue})");
        }
    }
}
