using UnityEngine;

/// <summary>
/// Loại tiền tệ trong game.
/// Hiện tại game dùng 2 loại:
/// - Gold: tiền trong run, reset mỗi lần chơi.
/// - Gem: tiền meta, dùng để nâng cấp lâu dài.
/// </summary>
public enum CurrencyType
{
    Gold = 0,
    Gem = 1
    // Sau này nếu có thêm PremiumCurrency thì cứ thêm vào đây.
}
