using LD60Tec.Commons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.FarmsSystem.Interactions
{
    public class PlaceWaterInteraction : InteractionBehaviour
    {
        [SerializeField]
        private FarmSystem _farm;
        public override void Interact(Player player)
        {
            Debug.Log($"[3] Held item: {(player.Inventory.GetFirst() ? player.Inventory.GetFirst().ItemName : "NONE")}");
            var water = player.Inventory.GetFirst();
            if (water == null)
            {
                print("No item"); return;
            }
            if (water.ItemName == "Water bucket")
            {
                _farm.PutWater();
                player.Inventory.RemoveFirst();
            }
            else
            {
                print("Its not water bucket");
            }
        }
    }
}
