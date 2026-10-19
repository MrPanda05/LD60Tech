using LD60Tec.Commons;
using LD60Tec.Commons.Singletons;
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
        [SerializeField]
        private GameObject _audioSucess;
        [SerializeField]
        private GameObject _audioFail;
        public override void Interact(Player player)
        {
            var item = player.Inventory.GetFirst();
            if (item == null)
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("Nothing to store");
                return;
            }

            if (!IsAllowed(item))
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print($"{item.ItemName} is not allowed here");
                return;
            }

            if (StorageManager.Instance.AddItem(item) > 0)
            {
                SoundPool.PlayAt(_audioSucess, transform.position);
                player.Inventory.RemoveFirst();
            }
            else
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("Storage is full");
            }
        }

        public bool IsAllowed(ItensSO item)
        {
            return _allowedItems.Count == 0 || _allowedItems.Contains(item);
        }
    }
}
