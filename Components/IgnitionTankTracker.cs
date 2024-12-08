using System.Linq;
using System.Collections.Generic;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class IgnitionTankTracker: MonoBehaviour
	{
		private CharacterBody body;
		//nothing bad ever happens to a static array
		private CharacterMaster[] TickMasterArray = new CharacterMaster[100];
		private int[] TickCountArray = new int[100];
		private void Awake()
		{
			body = GetComponent<CharacterBody>();
		}
		private void FixedUpdate()
		{
			if (!body)
			{
				Destroy(this);
				return;
			}
		}
		public bool TickUp(CharacterMaster attacker)
		{
			if (attacker)
			{
				int index = GetMasterIndex(attacker);
				if (index < 0)
				{
					index = AssignMasterIndex(attacker);
				}
				if (index > -1)
				{
					if (TickUp(index))
					{
						return true;
					}
				}
			}
			return false;
		}
		public bool TickUp(int index)
		{
			TickCountArray[index]++;
			if (TickCountArray[index] > Items.IgnitionTank_Rework.BlastTicks)
			{
				TickCountArray[index] -= Items.IgnitionTank_Rework.BlastTicks;
				return true;
			}
			return false;
		}
		private int GetMasterIndex(CharacterMaster master)
        {
			for (int i = 0; i < TickMasterArray.Length; i++)
			{
				if (TickMasterArray[i] == master)
				{
					return i;
				}
			}
			return -1;
		}
		private int AssignMasterIndex(CharacterMaster master)
		{
			for (int i = 0; i < TickMasterArray.Length; i++)
			{
				if (!TickMasterArray[i] || TickMasterArray[i] == null)
				{
					TickMasterArray[i] = master;
					TickCountArray[i] = Items.IgnitionTank_Rework.HalfBlastTicks;
					return i;
				}
			}
			return -1;
		}
	}
}
