using LD60Tec.Commons;
using LD60Tec.Commons.Singletons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.FarmsSystem.Interactions
{
    public class PlaceSeedInteraction : InteractionBehaviour
    {

        [SerializeField]
        private FarmSystem _farm;
        [SerializeField]
        private GameObject _audioSucess;
        [SerializeField]
        private GameObject _audioFail;
        public override void Interact(Player player)
        {
            var seed = player.Inventory.GetFirst();
            if (seed == null)
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("No seed"); return;
            }
            if(seed.GetType() == typeof(SeedSO))
            {
                SoundPool.PlayAt(_audioSucess, transform.position);
                _farm.PlantSeed(seed as SeedSO);
                player.Inventory.RemoveFirst();
            }
            else
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                print("Item is not seed");
            }
        }
    }
}
