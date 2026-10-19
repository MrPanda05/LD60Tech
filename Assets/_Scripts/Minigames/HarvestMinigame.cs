using LD60Tec.Commons.Singletons;
using LD60Tec.Minigames.Harvest;
using System;
using UnityEngine;

namespace LD60Tec.Minigames
{
    public class HarvestMinigame : MonoBehaviour
    {
        [SerializeField]
        private Waypoints _points;
        [SerializeField]
        private Fruit _fruit;
        [SerializeField]
        private int _count = 0;

        private void OnEnable()
        {
            BaseMinigame.OnStart?.Invoke();
        }
        private void OnDisable()
        {
        }
        public void StartMinigame(SeedSO seed)
        {
            var result = seed.ResultPlant;
            foreach (var p in _points.WayPoints)
            {
                var instance = Instantiate(_fruit);
                instance.OnFruitClick += FruitCount;
                instance.transform.position = p.transform.position;
                instance.Spriterender.sprite = result.ItemSprite;
            }
        }
        private void Check()
        {
            if(_count == _points.WayPoints.Count)
            {
                print("Gathered all");
                EndMinigame();
            }
        }
        private void FruitCount(Fruit fruit)
        {
            _count++;
            fruit.OnFruitClick -= FruitCount;
            Check();
            Destroy(fruit.gameObject);
        }

        private void EndMinigame()
        {
            BaseMinigame.OnEnd?.Invoke();
            _count = 0;
            MinigameStarter.Instance.MoveCameraToKitchen();
        }
    }
}
