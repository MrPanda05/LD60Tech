using LD60Tec.Commons;
using LD60Tec.Commons.Singletons;
using LD60Tec.LePlayer;
using UnityEngine;

namespace LD60Tec.Kitchen.Interactions
{
    public class FillWaterInteraction : InteractionBehaviour
    {
        public override void Interact(Player player)
        {
            if(player == null) return;
            var item = player.Inventory.GetFirst();
            if(item == null) return;
            if (player.Inventory.GetFirst().ItemName == "Bucket")
            {
                print("WeelMinigame");
                MinigameStarter.Instance.MoveCameraToWeel();
            }
        }
    }
}
