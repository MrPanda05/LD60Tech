using LD60Tec.LePlayer;
using System;
using UnityEngine;

namespace LD60Tec
{
    public class PausedUi : MonoBehaviour
    {

        public void Resume()
        {
            Player.Unpause();
        }
    }
}
