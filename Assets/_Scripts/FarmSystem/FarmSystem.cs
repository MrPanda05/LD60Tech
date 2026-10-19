using UnityEngine;

namespace LD60Tec.FarmsSystem
{
    public class FarmSystem : MonoBehaviour
    {
        [SerializeField]
        private bool _hasSeeds = false;
        [SerializeField]
        private bool _needsWater = false;
        [SerializeField]
        private bool _readyToHarvest = false;
        [SerializeField]
        private GameObject _seedsObj, _waterObj, _readyObj;
        [SerializeField]
        private SeedSO _currentSeed;
        public SeedSO CurrentSeed => _currentSeed;

        private void Awake()
        {
            Reset();
        }
        public void Reset()
        {
            _hasSeeds = false;
            _needsWater = false;
            _readyToHarvest = false;
            _seedsObj.SetActive(true);
            _waterObj.SetActive(false);
            _readyObj.SetActive(false);
            _currentSeed = null;
        }
        public void PlantSeed(SeedSO seed)
        {
            _currentSeed = seed;
            _hasSeeds = true;
            _seedsObj.SetActive(false);
            _waterObj.SetActive(true);
        }
        public void PutWater()
        {
            _needsWater = true;
            _waterObj.SetActive(false);
            _readyObj.SetActive(true);
        }
        public void Harvest()
        {
            Reset();
        }
    }
}
