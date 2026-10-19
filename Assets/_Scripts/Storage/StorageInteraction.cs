using LD60Tec.Commons;
using LD60Tec.LePlayer;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Storage
{
    public class StorageInteraction : InteractionBehaviour
    {
        [SerializeField]
        private List<ItensSO> _allowedItems = new();
        public override void Interact(Player player)
        {
            var item = player.Inventory.GetFirst();
            if (item == null)
            {
                print("Nothing to store");
                return;
            }

            if (!IsAllowed(item))
            {
                print($"{item.ItemName} is not allowed here");
                return;
            }

            if (StorageManager.Instance.AddItem(item) > 0)
            {
                player.Inventory.RemoveFirst();
            }
            else
            {
                print("Storage is full");
            }
        }

        public bool IsAllowed(ItensSO item)
        {
            return _allowedItems.Count == 0 || _allowedItems.Contains(item);
        }
    }
}
