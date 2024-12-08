using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class RaincoatBuffer: MonoBehaviour
	{
		private CharacterBody body;
		public float duration = 0f;

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
			duration -= Time.fixedDeltaTime;
		}

		public bool IsActive()
        {
			return duration > 0f;
		}
		public void Refresh()
		{
			duration = Items.BensRaincoat.GraceTime;
		}
	}
}
