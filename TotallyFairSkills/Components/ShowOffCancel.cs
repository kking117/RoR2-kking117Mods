using System;
using UnityEngine;
using RoR2;

namespace TotallyFairSkills.Components
{
	public class ShowOffCancel : MonoBehaviour
	{
		public DamageInfo damageInfo;
		public bool RemoveMe = false;
		private void FixedUpdate()
		{
			if (RemoveMe)
			{
				Destroy(this);
			}
			RemoveMe = true;
		}
		public void Remove()
		{
			Destroy(this);
		}
	}
}
