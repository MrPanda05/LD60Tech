using LD60Tec.Commons.Singletons;
using LD60Tec.Minigames.Well;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LD60Tec.Minigames
{
    public class WeelMinigame : BaseMinigame
    {
        [SerializeField]
        private CrankValve _valve;
        public static event Action OnWeelMinigameFinished;
        public static event Action OnWeelMinigameStart;
        [SerializeField]
        private ItensSO newItem;
        private void OnEnable()
        {
            BaseMinigame.OnStart?.Invoke();
            OnWeelMinigameStart?.Invoke();
            _valve.Reset();
            _valve.OnFull += EndMinigame;
        }
        private void OnDisable()
        {
            _valve.OnFull -= EndMinigame;
        }

        private void EndMinigame()
        {
            BaseMinigame.OnEnd?.Invoke();
            OnWeelMinigameFinished?.Invoke();
            InventoryFacade.Instance.Inventory.MutateFirst(newItem);
            MinigameStarter.Instance.MoveCameraToKitchen();
        }
    }
}
