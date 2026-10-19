using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
	public class NovaOnHealBatch: MonoBehaviour
	{
		private HealthComponent hpComp = null;
		public float healAmount = 0f;
		public float duration = 0.2f;

		private void Awake()
		{
			hpComp = GetComponent<HealthComponent>();
		}
		public void DoHeal()
		{
			if (healAmount > 0f)
            {
				ProcChainMask procChain = new ProcChainMask();
				procChain.AddProc(ProcType.SharedSuffering);
				hpComp.Heal(healAmount, procChain, true);
			}
			healAmount = 0f;
		}
	}
}
