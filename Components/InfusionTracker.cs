using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class InfusionTracker: MonoBehaviour
	{
		private CharacterBody body;
		public bool negateLevelBonus = true;
		public int recordLevel = 0;
		public int lastLevel = 0;
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
	}
}
