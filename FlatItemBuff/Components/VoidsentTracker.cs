using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class VoidsentTracker: MonoBehaviour
	{
		public List<CharacterMaster> HitList = new List<CharacterMaster>();
		public bool RegisterAttacker(CharacterMaster attacker)
        {
			CleanList();
			if (HitList.Contains(attacker))
            {
				return false;
            }
			HitList.Add(attacker);
			return true;
		}
		private void CleanList()
        {
			for(int i= 0;i<HitList.Count; i++)
            {
				if(HitList[i] == null)
                {
					HitList.RemoveAt(i);
				}
            }
        }
	}
}
