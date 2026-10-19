using LD60Tec.Minigames;
using Unity.Cinemachine;
using UnityEngine;

namespace LD60Tec.Commons.Singletons
{
    public class MinigameStarter : MonoBehaviour
    {
        public static MinigameStarter Instance { get; private set; }
        [SerializeField]
        private GameObject _kitchen, _wellMini, _harvestMini;
        [SerializeField]
        private HarvestMinigame _harvestMinigame;
        [SerializeField]
        private CinemachineCamera _cam1, _cam2, _cam3;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            MoveCameraToKitchen();
        }
        public void MoveCameraToKitchen()
        {
            _cam1.Priority = 100;
            _cam2.Priority = 0;
            _cam3.Priority = 0;
            _kitchen.SetActive(true);
            _wellMini.SetActive(false);
            _harvestMini.SetActive(false);
        }
        public void MoveCameraToWeel()
        {
            _cam1.Priority = 0;
            _cam2.Priority = 100;
            _cam3.Priority = 0;
            _kitchen.SetActive(false);
            _wellMini.SetActive(true);
            _harvestMini.SetActive(false);
        }
        public void StartHarvest(SeedSO seed)
        {
            _cam1.Priority = 0;
            _cam2.Priority = 0;
            _cam3.Priority = 100;
            _kitchen.SetActive(false);
            _wellMini.SetActive(false);
            _harvestMini.SetActive(true);
            _harvestMinigame.StartMinigame(seed);
        }
    }
}
