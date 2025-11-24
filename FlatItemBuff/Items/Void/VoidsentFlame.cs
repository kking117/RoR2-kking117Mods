using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Components;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.Items
{
	public class VoidsentFlame
	{
		private const string LogName = "Voidsent Flame";
		internal static bool Enable = false;
		internal static float BaseRadius = 12f;
		internal static float StackRadius = 2.4f;
		internal static float BaseDamage = 2.6f;
		internal static float StackDamage = 1.56f;
		internal static float ProcRate = 1f;
		public VoidsentFlame()
		{
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateItemDef();
			UpdateText();
			Hooks();
		}
		private void ClampConfig()
		{
			BaseDamage = Math.Max(0f, BaseDamage);
			StackDamage = Math.Max(0f, StackDamage);
			BaseRadius = Math.Max(0f, BaseRadius);
			StackRadius = Math.Max(0f, StackRadius);
			ProcRate = Math.Max(0f, ProcRate);
		}
		private void UpdateItemDef()
		{
			//"RoR2/DLC1/ExplodeOnDeathVoid/ExplodeOnDeathVoid.asset"
			ItemDef itemDef = Addressables.LoadAssetAsync<ItemDef>("e7bdf6730acde3648b085cf125ad4b72").WaitForCompletion();
			if (itemDef)
			{
				List<ItemTag> itemtags = itemDef.tags.ToList();
				itemtags.Add(ItemTag.CannotCopy);
				itemDef.tags = itemtags.ToArray();
			}
		}
		private void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string pickup = "Detonate an enemy on your first hit against them. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";
			string stackA = "";
			if (StackRadius != 0)
            {
				stackA = String.Format(" <style=cStack>(+{0}m per stack)</style>", StackRadius);
			}
			string stackB = "";
			if (StackDamage != 0)
			{
				stackB = String.Format(" <style=cStack>(+{0}% per stack)</style>", StackDamage * 100f);
			}
			string desc = String.Format("Upon hitting an enemy for the <style=cIsDamage>first time</style>, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>{0}m</style>{1} radius for <style=cIsDamage>{2}%</style>{3} base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.", BaseRadius, stackA, BaseDamage * 100f, stackB);
			LanguageAPI.Add("ITEM_EXPLODEONDEATHVOID_PICKUP", pickup);
			LanguageAPI.Add("ITEM_EXPLODEONDEATHVOID_DESC", desc);
		}
		private void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.HealthComponent.TakeDamageProcess += new ILContext.Manipulator(IL_TakeDamage);
		}
		private void IL_TakeDamage(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			//Remove max health condition
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(4),
				x => x.MatchLdarg(0)
			)
			&&
			ilcursor.TryGotoNext(
				x => x.MatchLdloc(4),
				x => x.MatchLdarg(0)
			))
			{
				ilcursor.Next.OpCode = OpCodes.Ldc_I4_1;
				ilcursor.Index += 1;
				ilcursor.RemoveRange(2);
				ilcursor.Emit(OpCodes.Ldc_I4_0);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_TakeDamage A - Hook failed");
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(1),
				x => x.MatchCallvirt(typeof(CharacterMaster), "get_inventory"),
				x => x.MatchLdsfld(typeof(DLC1Content.Items), "ExplodeOnDeathVoid"),
				x => x.MatchCallvirt(typeof(Inventory), "GetItemCountEffective"),
				x => x.MatchStloc(37)
			))
			{
				//Add new condition
				ilcursor.RemoveRange(4);
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.Emit(OpCodes.Ldarg, 1);
				ilcursor.EmitDelegate<Func<HealthComponent, DamageInfo, int>>((self, damageInfo) =>
				{
					VoidsentTracker tracker = self.GetComponent<VoidsentTracker>();
					if (!tracker)
					{
						tracker = self.gameObject.AddComponent<VoidsentTracker>();
					}
					CharacterBody victimBody = self.body;
					CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
					if (tracker.RegisterAttacker(attackerBody.master))
					{
						int itemCount = attackerBody.inventory.GetItemCountEffective(DLC1Content.Items.ExplodeOnDeathVoid);
						if (itemCount > 0f)
						{
							TeamIndex teamIndex = attackerBody.teamComponent.teamIndex;
							itemCount = Math.Max(0, itemCount - 1);
							Vector3 corePosition = Util.GetCorePosition(victimBody);
							float damage = BaseDamage + (StackDamage * itemCount);
							damage *= attackerBody.damage;
							float radius = BaseRadius + (StackRadius * itemCount);

							GameObject blastPrefab = UnityEngine.Object.Instantiate<GameObject>(HealthComponent.AssetReferences.explodeOnDeathVoidExplosionPrefab, corePosition, Quaternion.identity);
							DelayBlast delayBlast = blastPrefab.GetComponent<DelayBlast>();
							delayBlast.position = corePosition;
							delayBlast.baseDamage = damage;
							delayBlast.procCoefficient = ProcRate;
							delayBlast.baseForce = 1000f;
							delayBlast.radius = radius;
							delayBlast.attacker = damageInfo.attacker;
							delayBlast.inflictor = null;
							delayBlast.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
							delayBlast.maxTimer = 0.2f;
							delayBlast.damageColorIndex = DamageColorIndex.Void;
							delayBlast.falloffModel = BlastAttack.FalloffModel.SweetSpot;
							delayBlast.GetComponent<TeamFilter>().teamIndex = teamIndex;
							NetworkServer.Spawn(blastPrefab);
						}
					}
					return 0;
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_TakeDamage B - Hook failed");
			}
		}
	}
}
