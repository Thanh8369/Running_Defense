using System;
using UnityEngine;

/// <summary>
/// Dữ liệu lưu trữ cho Wallet.
/// Hiện tại game chỉ cần lưu Gem (meta currency).
/// Sau này nếu muốn lưu thêm (vd: Premium, Token...), chỉ cần thêm field.
/// </summary>
[Serializable]
public class WalletSaveData
{
    public int gem;

    public override string ToString()
    {
        return $"WalletSaveData (Gem={gem})";
    }
}