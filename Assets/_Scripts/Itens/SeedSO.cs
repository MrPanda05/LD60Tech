using UnityEngine;

namespace LD60Tec
{
    [CreateAssetMenu(fileName = "SeedSO", menuName = "Scriptable Objects/SeedSO")]
    public class SeedSO : ItensSO
    {
        [field: SerializeField]
        public ItensSO ResultPlant {  get; set; }
    }
}
