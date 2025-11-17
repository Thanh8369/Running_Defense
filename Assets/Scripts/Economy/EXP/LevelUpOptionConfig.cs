using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Nâng cấp khi Level Up (miễn phí).
    /// Bạn sẽ code logic ApplyEffect() theo game của bạn.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelUpOption", menuName = "Son/Economy/Level Up Option", order = 10)]
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
        /// Thực thi hiệu ứng nâng cấp. Ở đây mình chỉ log, bạn tự nối vào hệ thống combat / stat.
        /// </summary>
        public virtual void ApplyEffect()
        {
            Debug.Log($"[LevelUpOption] ApplyEffect: {id} - {displayName} (power={powerValue})");
            // TODO: gọi vào PlayerStats / CombatManager... tuỳ game của bạn
        }
    }
}
