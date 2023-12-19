using System;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Components
{
	public class IgnitionTankTicker : MonoBehaviour
	{
		private CharacterBody body;
		private int TickCount = 0;
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
		public bool TickUp()
		{
			TickCount++;
			if (TickCount > Items.IgnitionTank_Rework.BlastTicks)
            {
				TickCount -= Items.IgnitionTank_Rework.BlastTicks + UnityEngine.Random.Range(-1, 1);
				return true;
			}
			return false;
		}
	}
}
