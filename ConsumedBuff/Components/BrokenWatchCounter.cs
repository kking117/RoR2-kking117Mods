using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ConsumedBuff.Components
{
    public class BrokenWatchCounter : MonoBehaviour
	{
        public int hits = 0;
        public float LastHitFrame = -1f;
        public int hitmult = 0;
    }
}
