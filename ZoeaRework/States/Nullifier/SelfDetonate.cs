using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ZoeaRework.States.Nullifier
{
    public class SelfDetonate : BaseSkillState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			if(NetworkServer.active)
            {
				if(healthComponent)
                {
					DamageInfo dmginfo = new DamageInfo();
					dmginfo.damage = healthComponent.fullCombinedHealth * 3;
					dmginfo.position = characterBody.corePosition;
					dmginfo.force = Vector3.zero;
					dmginfo.crit = false;
					dmginfo.attacker = null;
					dmginfo.inflictor = null;
					dmginfo.damageType = (DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection | DamageType.Silent);
					characterBody.healthComponent.TakeDamage(dmginfo);
				}
            }
			outer.SetNextStateToMain();
		}
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
