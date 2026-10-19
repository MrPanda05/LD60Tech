using System;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Storage
{
    /// <summary>
    /// One slot of the chest. Plain data: the UI reads it, the StorageManager owns it.
    /// </summary>
    public class InventorySlot
    {
        public int Id { get; }
        public ItensSO Item { get; internal set; }
        public int ItemCount { get; internal set; }

        public bool IsEmpty => Item == null || ItemCount <= 0;

        public InventorySlot(int id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// A chest: a fixed number of slots, each holding a stack of one item type.
    /// Keep it on an always-active object so it survives the kitchen/minigame toggling.
    /// </summary>
    public class StorageManager : MonoBehaviour
    {
        public static StorageManager Instance { get; private set; }

        [SerializeField, Min(1)]
        private int _slotCount = 6;
        [SerializeField, Min(1), Tooltip("Max items of the same type in one slot.")]
        private int _maxStackSize = 10;

        // ---- Events (plain C# Actions, for the UI) ----
        public event Action<InventorySlot> OnSlotChanged;      // item added/removed in a slot
        public event Action OnSlotsRebuilt;                    // slot count changed (build the whole UI again)
        public event Action<ItensSO, int> OnAddRejected;       // (item, amount that did not fit)

        private readonly List<InventorySlot> _slots = new List<InventorySlot>();

        public IReadOnlyList<InventorySlot> Slots => _slots;
        public int MaxStackSize => _maxStackSize;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BuildSlots();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void BuildSlots()
        {
            _slots.Clear();
            for (int i = 0; i < _slotCount; i++)
            {
                _slots.Add(new InventorySlot(i));
            }
        }

        // ---- Queries ----

        public InventorySlot GetSlot(int id)
        {
            return id >= 0 && id < _slots.Count ? _slots[id] : null;
        }

        public int CountOf(ItensSO item)
        {
            if (item == null) return 0;

            int total = 0;
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty && slot.Item == item) total += slot.ItemCount;
            }
            return total;
        }

        public bool Has(ItensSO item, int amount = 1)
        {
            return CountOf(item) >= amount;
        }

        /// <summary>True if the whole amount fits (existing stacks first, then empty slots).</summary>
        public bool CanAdd(ItensSO item, int amount = 1)
        {
            if (item == null || amount <= 0) return false;

            int space = 0;
            foreach (var slot in _slots)
            {
                if (slot.IsEmpty) space += _maxStackSize;
                else if (slot.Item == item) space += _maxStackSize - slot.ItemCount;
            }
            return space >= amount;
        }

        // ---- Changes ----

        /// <summary>
        /// Adds as many as fit. Returns how many were actually added; the rest raises OnAddRejected.
        /// </summary>
        public int AddItem(ItensSO item, int amount = 1)
        {
            if (item == null || amount <= 0) return 0;

            int remaining = amount;

            // 1) Top up stacks that already hold this item.
            foreach (var slot in _slots)
            {
                if (remaining == 0) break;
                if (slot.IsEmpty || slot.Item != item) continue;
                remaining -= Fill(slot, item, remaining);
            }

            // 2) Use empty slots for the rest.
            foreach (var slot in _slots)
            {
                if (remaining == 0) break;
                if (!slot.IsEmpty) continue;
                remaining -= Fill(slot, item, remaining);
            }

            if (remaining > 0)
            {
                OnAddRejected?.Invoke(item, remaining);
            }
            return amount - remaining;
        }

        /// <summary>
        /// Removes exactly this amount of an item, or nothing at all.
        /// Takes from the smallest stacks first so slots free up sooner.
        /// </summary>
        public bool TryRemove(ItensSO item, int amount = 1)
        {
            if (item == null || amount <= 0 || !Has(item, amount)) return false;

            int remaining = amount;
            while (remaining > 0)
            {
                InventorySlot smallest = null;
                foreach (var slot in _slots)
                {
                    if (slot.IsEmpty || slot.Item != item) continue;
                    if (smallest == null || slot.ItemCount < smallest.ItemCount) smallest = slot;
                }

                remaining -= Take(smallest, remaining);
            }
            return true;
        }

        /// <summary>
        /// Takes up to 'amount' from one specific slot (what a "click this slot" UI would call).
        /// Returns false if the slot is empty.
        /// </summary>
        public bool TryTakeFromSlot(int slotId, int amount, out ItensSO item, out int taken)
        {
            item = null;
            taken = 0;

            var slot = GetSlot(slotId);
            if (slot == null || slot.IsEmpty || amount <= 0) return false;

            item = slot.Item;
            taken = Take(slot, amount);
            return true;
        }

        /// <summary>For upgrades like "bigger chest".</summary>
        public void AddSlots(int extra)
        {
            if (extra <= 0) return;

            for (int i = 0; i < extra; i++)
            {
                _slots.Add(new InventorySlot(_slots.Count));
            }
            OnSlotsRebuilt?.Invoke();
        }

        public void Clear()
        {
            foreach (var slot in _slots)
            {
                if (slot.IsEmpty) continue;
                slot.Item = null;
                slot.ItemCount = 0;
                OnSlotChanged?.Invoke(slot);
            }
        }

        // ---- Helpers ----

        private int Fill(InventorySlot slot, ItensSO item, int amount)
        {
            int current = slot.IsEmpty ? 0 : slot.ItemCount;
            int moved = Mathf.Min(_maxStackSize - current, amount);
            if (moved <= 0) return 0;

            slot.Item = item;
            slot.ItemCount = current + moved;
            OnSlotChanged?.Invoke(slot);
            return moved;
        }

        private int Take(InventorySlot slot, int amount)
        {
            int taken = Mathf.Min(slot.ItemCount, amount);
            slot.ItemCount -= taken;
            if (slot.ItemCount <= 0)
            {
                slot.ItemCount = 0;
                slot.Item = null;
            }
            OnSlotChanged?.Invoke(slot);
            return taken;
        }
    }
}