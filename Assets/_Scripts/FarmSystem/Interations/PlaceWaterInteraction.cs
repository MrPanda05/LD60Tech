using LD60Tec.Commons;
using LD60Tec.Commons.Singletons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.FarmsSystem.Interactions
{
    public class PlaceWaterInteraction : InteractionBehaviour
    {
        [SerializeField]
        private FarmSystem _farm;
        [SerializeField]
        private GameObject _audioSucess;
        [SerializeField]
        private GameObject _audioFail;
        public override void Interact(Player player)
        {
            Debug.Log($"[3] Held item: {(player.Inventory.GetFirst() ? player.Inventory.GetFirst().ItemName : "NONE")}");
            var water = player.Inventory.GetFirst();
            if (water == null)
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("No item"); return;
            }
            if (water.ItemName == "Water bucket")
            {
                SoundPool.PlayAt(_audioSucess, transform.position);
                _farm.PutWater();
                player.Inventory.RemoveFirst();
            }
            else
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("Its not water bucket");
            }
        }
    }
}
