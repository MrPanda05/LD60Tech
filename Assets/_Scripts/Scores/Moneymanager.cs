using UnityEngine;

namespace LD60Tec.Score
{
    public class Moneymanager : MonoBehaviour
    {
        public static Moneymanager Instance { get; private set; }
        public int CurrentMoney = 0;
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
            CurrentMoney = 0;
        }

        public void IncreaseMoney(int amount)
        {
            CurrentMoney += amount;
        }
    }
}
