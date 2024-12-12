using System;
using RoR2;
using R2API;

namespace FlatItemBuff.Items
{
	public class PocketICBM
	{
		private const string LogName = "Pocket ICBM";
		internal static bool Enable = false;
		internal static float BaseChance = 7f;
		internal static float StackChance = 0f;
		internal static float BaseDamage = 2f;
		internal static float StackDamage = 0f;
		public PocketICBM()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseChance = Math.Max(0f, BaseChance);
			StackChance = Math.Max(0f, StackChance);
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			//string pickup = "";
			string description = "Missile items, equipment and skills fire an additional <style=cIsDamage>2</style> missiles and deal <style=cIsDamage>0%</style> <style=cStack>(+50% per stack)</style> more damage.";
			if (BaseChance > 0f && BaseDamage > 0f)
            {
				if (StackChance > 0f)
                {
					description += string.Format(" <style=cIsDamage>{0}%</style> <style=cStack>(+{1}% per stack)</style> chance on hit to fire a missile", BaseChance, StackChance);
				}
				else
                {
					description += string.Format(" <style=cIsDamage>{0}%</style> chance on hit to fire a missile", BaseChance);
				}
				if (StackDamage > 0f)
                {
					description += string.Format(" that deals <style=cIsDamage>{0}%</style> <style=cStack>(+{1}% per stack)</style> TOTAL damage.", BaseDamage * 100f, StackDamage * 100f);
                }
				else
                {
					description += string.Format(" that deals <style=cIsDamage>{0}%</style> TOTAL damage.", BaseDamage * 100f);
				}					
			}
			//LanguageAPI.Add("ITEM_MOREMISSILE_DESC", pickup);
			LanguageAPI.Add("ITEM_MOREMISSILE_DESC", description);
		}
		private void Hooks()
		{
			if (BaseChance > 0f && BaseDamage > 0f)
			{
				SharedHooks.Handle_GlobalHitEvent_Actions += GlobalEventManager_HitEnemy;
			}
		}
		private void GlobalEventManager_HitEnemy(CharacterBody victimBody, CharacterBody attackerBody, DamageInfo damageInfo)
		{
			if (damageInfo.procCoefficient <= 0f || damageInfo.rejected)
			{
				return;
			}
			if (damageInfo.procChainMask.HasProc(ProcType.Missile))
            {
				return;
            }
			if (attackerBody && attackerBody.master && attackerBody.inventory)
            {
				int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
				if (itemCount > 0)
                {
					itemCount = Math.Max(0, itemCount - 1);
					float effectChance = BaseChance + (itemCount * StackChance);
					if (Util.CheckRoll(effectChance * damageInfo.procCoefficient, attackerBody.master))
                    {
						float dmgMult = BaseDamage + (itemCount * StackDamage);
						dmgMult = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, dmgMult);
						MissileUtils.FireMissile(attackerBody.corePosition, attackerBody, damageInfo.procChainMask, victimBody.gameObject, dmgMult, damageInfo.crit, GlobalEventManager.CommonAssets.missilePrefab, DamageColorIndex.Item, true);
					}
				}
            }
		}
	}
}
