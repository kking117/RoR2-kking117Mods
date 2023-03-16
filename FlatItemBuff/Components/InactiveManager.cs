using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class InactiveManager : MonoBehaviour
	{
		private CharacterMaster master;
		public float MaxLifeTime = 30f;
		private float timer;
		private void Awake()
		{
			master = GetComponent<CharacterMaster>();
			timer = 0f;
			MaxLifeTime = 30f;
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
							master.TrueKill();
							Destroy(this);
							return;
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
