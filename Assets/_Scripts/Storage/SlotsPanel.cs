using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Storage
{
    /// <summary>
    /// Shows one SlotsBlock per occupied chest slot. Put it on the object that has the Vertical Layout Group.
    /// The panel should contain only these blocks, because it orders them by slot id using sibling indexes.
    /// </summary>
    public class SlotsPanel : MonoBehaviour
    {
        [SerializeField]
        private SlotsBlock _block;

        // slot id -> its block
        private readonly Dictionary<int, SlotsBlock> _blocks = new Dictionary<int, SlotsBlock>();

        private void Start()
        {
            var storage = StorageManager.Instance;
            storage.OnSlotChanged += HandleSlotChanged;
            storage.OnSlotsRebuilt += Rebuild;

            Rebuild(); // show whatever is already in the chest
        }

        private void OnDestroy()
        {
            var storage = StorageManager.Instance;
            if (storage == null) return; // already torn down

            storage.OnSlotChanged -= HandleSlotChanged;
            storage.OnSlotsRebuilt -= Rebuild;
        }

        // ---- Create / Update / Remove ----

        private void HandleSlotChanged(InventorySlot slot)
        {
            bool hasBlock = _blocks.TryGetValue(slot.Id, out var block);

            if (slot.IsEmpty)
            {
                if (hasBlock) RemoveBlock(slot.Id);
            }
            else if (hasBlock)
            {
                block.Refresh();
            }
            else
            {
                CreateBlock(slot);
            }
        }

        private void CreateBlock(InventorySlot slot)
        {
            var block = Instantiate(_block, transform); // parent set here, so the layout group picks it up
            block.Bind(slot);
            _blocks.Add(slot.Id, block);

            // Keep blocks in slot order even when a lower slot fills up later.
            block.transform.SetSiblingIndex(SiblingIndexFor(slot.Id));
        }

        private void RemoveBlock(int slotId)
        {
            if (!_blocks.TryGetValue(slotId, out var block)) return;

            _blocks.Remove(slotId);
            if (block != null)
            {
                block.gameObject.SetActive(false); // so the layout group closes the gap right away
                Destroy(block.gameObject);
            }
        }

        private void Rebuild()
        {
            foreach (var block in _blocks.Values)
            {
                if (block != null) Destroy(block.gameObject);
            }
            _blocks.Clear();

            foreach (var slot in StorageManager.Instance.Slots)
            {
                if (!slot.IsEmpty) CreateBlock(slot);
            }
        }

        private int SiblingIndexFor(int slotId)
        {
            int index = 0;
            foreach (var pair in _blocks)
            {
                if (pair.Key < slotId) index++;
            }
            return index;
        }
    }
}