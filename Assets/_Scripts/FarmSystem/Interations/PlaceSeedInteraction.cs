using LD60Tec.Commons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.FarmsSystem.Interactions
{
    public class PlaceSeedInteraction : InteractionBehaviour
    {

        [SerializeField]
        private FarmSystem _farm;
        public override void Interact(Player player)
        {
            var seed = player.Inventory.GetFirst();
            if (seed == null)
            {
                print("No seed"); return;
            }
            if(seed.GetType() == typeof(SeedSO))
            {
                _farm.PlantSeed(seed as SeedSO);
                player.Inventory.RemoveFirst();
            }
            else
            {
                print("Item is not seed");
            }
        }
    }
}
