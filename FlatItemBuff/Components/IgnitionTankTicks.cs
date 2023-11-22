using System;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Components
{
	public class IgnitionTankTicks : MonoBehaviour
	{
		private CharacterBody body;
		private int[] TickList = new int[(int)DotController.DotIndex.Count];
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
		public bool TickUpDoT(DotController.DotIndex dotIndex)
		{
			int arrayIndex = (int)dotIndex;
			if (arrayIndex < 0)
            {
				return false;
            }
			if (arrayIndex >= TickList.Length)
            {
				Array.Resize(ref TickList, arrayIndex+1);
			}
			TickList[arrayIndex]++;
			if (TickList[arrayIndex] > Items.IgnitionTank_Rework.BlastTicks)
            {
				TickList[arrayIndex] -= Items.IgnitionTank_Rework.BlastTicks;
				return true;
			}
			return false;
		}
	}
}
