using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace LD60Tec.Minigames.Harvest
{
    public class Waypoints : MonoBehaviour
    {
        [field:SerializeField]
        public List<Transform> WayPoints { get; private set; }
    }
}
