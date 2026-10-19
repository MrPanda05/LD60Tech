using LD60Tec.LePlayer;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Commons.Components
{
    /// <summary>
    /// Detects interactions
    /// </summary>
    public class InteractorComponent : MonoBehaviour
    {
        [SerializeField]
        private float _radios = 5;
        [SerializeField]
        private LayerMask _mask;
        public bool DetectInteraction()
        {
            if(Physics2D.OverlapCircle(transform.position, _radios, _mask))
            {
                return true;
            }
            return false;
        }
        public InteractionComponent GetInteraction()
        {
            if (!DetectInteraction()) return null;
            Collider2D col = Physics2D.OverlapCircle(transform.position, _radios, _mask);
            if (col != null && col.TryGetComponent(out InteractionComponent interaction))
            {
                return interaction;
            }
            return null;
        }
        public void UseInteraction(Player player)
        {
            var interaction = GetInteraction();
            Debug.Log($"[2] Interaction found: {(interaction ? interaction.name : "NONE")}");
            if (interaction != null)
            {
                interaction.Use(player);
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = DetectInteraction() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, _radios);
        }
#endif
    }
}
