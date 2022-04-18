using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class DisableHealManager : MonoBehaviour
	{
		private CharacterMaster master;
		public float MaxLifeTime = 1f;
		private float timer;

		private void Awake()
		{
			master = GetComponent<CharacterMaster>();
			timer = 0f;
			MaxLifeTime = MainPlugin.Squid_InactiveDecay.Value;
		}
		private void FixedUpdate()
		{
			if (!master)
			{
				Destroy(this);
				return;
			}
			if (NetworkServer.active)
			{
				CharacterBody body = master.GetBody();
				if(body)
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
				else
                {
					timer = 0f;
				}
			}
		}
	}
}
