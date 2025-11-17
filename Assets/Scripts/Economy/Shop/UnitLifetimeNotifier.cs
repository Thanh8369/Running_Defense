using UnityEngine;

namespace Son.Economy
{
    /// <summary>
    /// Component gắn lên lính để báo về UnitPurchaseService khi unit bị Destroy.
    /// </summary>
    public class UnitLifetimeNotifier : MonoBehaviour
    {
        [HideInInspector] public string itemId;
        [HideInInspector] public UnitPurchaseService service;

        private void OnDestroy()
        {
            if (service != null && !string.IsNullOrEmpty(itemId))
            {
                service.NotifyUnitDied(itemId);
            }
        }
    }
}
