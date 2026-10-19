using UnityEngine;

namespace LD60Tec
{
    [CreateAssetMenu(fileName = "ItensSO", menuName = "Scriptable Objects/ItensSO")]
    public class ItensSO : ScriptableObject
    {
        [field:SerializeField]
        public string ItemName { get; set; }
        [field:SerializeField]
        public Sprite ItemSprite { get; set; }
    }
}
