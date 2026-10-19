using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LD60Tec.Storage
{
    public class SlotsBlock : MonoBehaviour
    {
        [field: SerializeField]
        public Image Render { get; private set; }
        [field: SerializeField]
        public TMP_Text BlockText { get; private set; }

        /// <summary>The slot this block is showing.</summary>
        public InventorySlot Slot { get; private set; }

        public void Bind(InventorySlot slot)
        {
            Slot = slot;
            Refresh();
        }

        /// <summary>Re-reads the slot and updates the icon and text.</summary>
        public void Refresh()
        {
            if (Slot == null || Slot.IsEmpty) return;

            Render.sprite = Slot.Item.ItemSprite;
            BlockText.text = $"{Slot.Item.ItemName} x{Slot.ItemCount}"; // change the format here
        }
    }
}