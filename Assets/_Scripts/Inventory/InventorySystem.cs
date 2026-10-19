using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Inventorys
{
    /// <summary>
    /// Will host a list of items the player has, visualized on top of its head, fifo
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [field: SerializeField]
        public int MaxSlots { get; private set; } = 1;

        [SerializeField]
        private Queue<ItensSO> _inventoryQueue = new Queue<ItensSO>();
        public Queue<ItensSO> Inventory => _inventoryQueue;

        public event Action<ItensSO> OnInventoryChanged;
        public event Action OnRebuild;

        [SerializeField]
        private ItensSO _firstItem;

        private void Update()
        {
            _firstItem = _inventoryQueue.Count != 0 ? _inventoryQueue.Peek() : null;
        }
        public bool AddItem(ItensSO item)
        {
            if(item == null) return false;
            if (_inventoryQueue.Count >= MaxSlots)
            {
                print("Sorry max out");
                return false;
            }
            print("Item added");
            _inventoryQueue.Enqueue(item);
            OnInventoryChanged?.Invoke(item);
            return true;
        }
        public void RemoveFirst()
        {
            if (_inventoryQueue.Count == 0) return;
            _inventoryQueue.Dequeue();
            OnRebuild?.Invoke();
        }
        public ItensSO GetFirst()
        {
            if (_inventoryQueue.Count == 0) return null;
            return _inventoryQueue.Peek();
        }
        public void MutateFirst(ItensSO newItem)
        {
            if (newItem == null)
            {
                Debug.LogWarning("MutateFirst called with a null item", this);
                return;
            }

            if (_inventoryQueue.Count == 0)
            {
                AddItem(newItem); // or just return, depending on your design
                return;
            }

            var items = _inventoryQueue.ToArray();
            items[0] = newItem;
            _inventoryQueue = new Queue<ItensSO>(items);
            OnRebuild?.Invoke();
        }
    }
}
