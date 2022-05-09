using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class SummonDeclutter : MonoBehaviour
	{
		private CharacterMaster master;
		public DeployableSlot slot;
		private float timer;
		private void Awake()
		{
			master = GetComponent<CharacterMaster>();
			timer = UnityEngine.Random.Range(0.25f, 0.5f);
		}
		private void FixedUpdate()
		{
			if (!master || !master.GetBody())
			{
				Destroy(this);
				return;
			}
			if (NetworkServer.active)
			{
				if(timer > 0f)
                {
					timer -= Time.fixedDeltaTime;
                }
				if(timer <= 0f)
                {
					Utils.Helpers.KillCloseDeployables(master, master.teamIndex, DeployableSlot.MinorConstructOnKill, master.GetBody().transform.position, 4f);
					Destroy(this);
				}
			}
		}
	}
}
