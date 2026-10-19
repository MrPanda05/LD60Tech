using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Commons.Interactions
{
    public class TrashInteraction : InteractionBehaviour
    {
        public override void Interact(Player player)
        {
            player.Inventory.RemoveFirst();
        }
    }
}
