using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Commons.Components
{
    /// <summary>
    /// Makes the object an interaction
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractionComponent : MonoBehaviour
    {
        private Collider2D _collider;
        [SerializeField]
        private InteractionBehaviour _interaction;
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            gameObject.layer = LayerMask.NameToLayer("Interaction");
            _collider.isTrigger = true;
        }
        public void Use(Player player)
        {
            if (_interaction != null)
            {
                _interaction.Interact(player);
            }
        }
    }
}
