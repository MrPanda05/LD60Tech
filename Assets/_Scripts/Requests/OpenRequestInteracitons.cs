using LD60Tec.Commons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Requests
{
    public class OpenRequestInteracitons : InteractionBehaviour
    {
        [SerializeField]
        private GameObject _canvas;
        private void Start()
        {
            _canvas.GetComponent<Canvas>().enabled = false;
        }
        public override void Interact(Player player)
        {
            player.DisableMovement();
            _canvas.GetComponent<Canvas>().enabled = true;
        }
    }
}
