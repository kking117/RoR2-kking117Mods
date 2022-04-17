using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class DisableHealManager : MonoBehaviour
	{
		private CharacterBody body;
		public float MaxLifeTime = 1f;
		private float timer;

		private void Awake()
		{
			body = GetComponent<CharacterBody>();
			if(body)
            {
				body.outOfCombat = false;
				body.outOfDanger = false;
			}
			timer = MainPlugin.Squid_InactiveDecay.Value;
		}
		private void FixedUpdate()
		{
			if (!body)
			{
				Destroy(this);
				return;
			}
			if (NetworkServer.active)
			{
				if (body.outOfCombat && body.outOfDanger)
				{
					if (timer > MaxLifeTime)
					{
						body.AddTimedBuff(RoR2Content.Buffs.HealingDisabled, 0.5f);
					}
					else
					{
						timer += Time.fixedDeltaTime;
					}
				}
				else
				{
					timer = 0f;
				}
			}
		}
	}
}
