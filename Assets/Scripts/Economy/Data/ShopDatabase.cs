using System.Collections.Generic;
using UnityEngine;

namespace Son.Economy
{
    [CreateAssetMenu(fileName = "ShopDatabase", menuName = "Son/Economy/Shop Database", order = 1)]
    public class ShopDatabase : ScriptableObject
    {
        public List<ShopItemConfig> items = new List<ShopItemConfig>();
    }
}
