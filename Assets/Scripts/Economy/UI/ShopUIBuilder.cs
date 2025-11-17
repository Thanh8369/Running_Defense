using UnityEngine;

namespace Son.Economy
{
    public class ShopUIBuilder : MonoBehaviour
    {
        public ShopDatabase database;
        public UpgradeButton itemPrefab;
        public Transform contentParent;

        private void Start()
        {
            if (database == null || itemPrefab == null || contentParent == null) return;

            foreach (var cfg in database.items)
            {
                var ui = Instantiate(itemPrefab, contentParent);
                ui.item = cfg;
            }
        }
    }
}
