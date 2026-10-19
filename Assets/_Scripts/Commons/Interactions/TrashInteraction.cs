using LD60Tec.Commons.Singletons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Commons.Interactions
{
    public class TrashInteraction : InteractionBehaviour
    {
        [SerializeField]
        private GameObject _audioSucess;
        [SerializeField]
        private GameObject _audioFail;
        public override void Interact(Player player)
        {
            if(player.Inventory.GetFirst() == null)
            {
                SoundPool.PlayAt(_audioFail, transform.position);
                return;
            }
            player.Inventory.RemoveFirst();
            SoundPool.PlayAt(_audioSucess, transform.position);
        }
    }
}
