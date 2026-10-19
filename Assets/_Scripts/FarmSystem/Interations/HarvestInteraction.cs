using LD60Tec.Commons;
using LD60Tec.Commons.Singletons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.FarmsSystem.Interactions
{
    public class HarvestInteraction : InteractionBehaviour
    {
        [SerializeField]
        private FarmSystem _farm;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        private void OnEnable()
        {
            _spriteRenderer.sprite = _farm.CurrentSeed.ResultPlant.ItemSprite;
        }
        public override void Interact(Player player)
        {
            player.Inventory.RemoveFirst();
            MinigameStarter.Instance.StartHarvest(_farm.CurrentSeed);
            player.Inventory.AddItem(_farm.CurrentSeed.ResultPlant);
            _farm.Harvest();
        }
    }
}
