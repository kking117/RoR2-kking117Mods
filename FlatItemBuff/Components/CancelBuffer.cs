using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class CancelBuffer: MonoBehaviour
	{
		private CharacterBody body;
		public float duration;

		private void Awake()
		{
			body = GetComponent<CharacterBody>();
			duration = 1.0f;
		}
		private void FixedUpdate()
		{
			if (!body || duration < 0.0f)
			{
				Destroy(this);
				return;
			}
			if (NetworkServer.active)
			{
				duration -= Time.fixedDeltaTime;
			}
		}
	}
}
