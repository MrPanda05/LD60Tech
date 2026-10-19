using LD60Tec.Commons.Singletons;
using LD60Tec.Inventorys;
using UnityEngine;

namespace LD60Tec
{
    public class InventoryFacade : MonoBehaviour
    {
        public static InventoryFacade Instance { get; private set; }
        [field:SerializeField]
        public InventorySystem Inventory { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
}
