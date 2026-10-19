using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Commons.Interactions
{
    public class ItemGiverInteraction : InteractionBehaviour
    {
        [SerializeField]
        private ItensSO _itemInstance;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private bool _isInfinite = false;
        private int _maxItems = 3;
        private int _itemCount;
        public int ItemCount => _itemCount;

        private void Start()
        {
            _itemCount = _maxItems;
            if (_spriteRenderer == null) return;
            _spriteRenderer.sprite = _itemInstance.ItemSprite;
        }
        public override void Interact(Player player)
        {
            if(!_isInfinite && _itemCount <= 0)
            {
                print("Items out of stock");
                return;
            }
            if (!_isInfinite)
            {
                _itemCount--;
            }
            print("Here is an item");
            player.Inventory.AddItem(_itemInstance);
        }
        public void Restock()
        {
            _itemCount = _maxItems;
        }
        public void IncreaseMaxItems(int newMaxItems)
        {
            _maxItems = newMaxItems;
        }
    }
}
