using RoR2;
using R2API;
using MonoMod.Cil;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace FlatItemBuff.ItemChanges
{
	public class TitanicKnurl_Rework
	{
		private static string IL_ItemName = "Knurl";
		private static int IL_LocationOffset = 2;
		private static int IL_Location = 16;

		public static GameObject StoneFistProjectile;
		public static void EnableChanges()
		{
			MainPlugin.ModLogger.LogInfo("Changing Titanic Knurl");
			UpdateText();
			CreateProjectiles();
			Hooks();
		}
		private static void UpdateText()
		{
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string pickup = string.Format("Every few seconds a Stone Titan attacks a nearby enemy.");
			string stackCool = "";
			if (MainPlugin.KnurlRework_StackSpeed.Value != 0f)
            {
				stackCool = string.Format(" <style=cStack>(+{0}% cooldown rate per stack)</style>", MainPlugin.KnurlRework_StackSpeed.Value * 100f);
			}
			string targetText;
			if (MainPlugin.KnurlRework_TargetType.Value == 0)
            {
				targetText = string.Format(" a weak enemy within <style=cIsUtility>{0}m</style>", MainPlugin.KnurlRework_AttackRange.Value);
			}
			else
            {
				targetText = string.Format(" a nearby enemy within <style=cIsUtility>{0}m</style>", MainPlugin.KnurlRework_AttackRange.Value);
			}
			string stackDamage = "";
			if (MainPlugin.KnurlRework_StackDamage.Value != 0f)
			{
				stackDamage = string.Format(" <style=cStack>(+{0}% per stack)</style>", MainPlugin.KnurlRework_StackDamage.Value * 100f);
			}
			string desc = string.Format("Every <style=cIsUtility>{0}</style> seconds{1}{2} is attacked by a <style=cIsDamage>Stone Titan's fist</style> for <style=cIsDamage>{3}%</style>{4} base damage.", MainPlugin.KnurlRework_BaseSpeed.Value, stackCool, targetText, MainPlugin.KnurlRework_BaseDamage.Value * 100f, stackDamage);
			LanguageAPI.Add("ITEM_KNURL_PICKUP", pickup);
			LanguageAPI.Add("ITEM_KNURL_DESC", desc);
		}
		private static void Hooks()
		{
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.CharacterBody.RecalculateStats += new ILContext.Manipulator(IL_RecalculateStats);
			CharacterBody.onBodyInventoryChangedGlobal += OnInventoryChanged;
		}
		private static void CreateProjectiles()
        {
			StoneFistProjectile = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanPreFistProjectile.prefab").WaitForCompletion(), MainPlugin.MODTOKEN + "StonePreFist");
			ProjectileDamage projDmg = StoneFistProjectile.GetComponent<ProjectileDamage>();
			ProjectileController projController = StoneFistProjectile.GetComponent<ProjectileController>();
			projController.procCoefficient = MainPlugin.KnurlRework_ProcRate.Value;
			projDmg.damageType = DamageType.Stun1s;
			Modules.Projectiles.AddProjectile(StoneFistProjectile);
		}
		private static void OnInventoryChanged(CharacterBody self)
		{
			if(NetworkServer.active)
            {
				self.AddItemBehavior<TitanicKnurl_Behavior>(self.inventory.GetItemCount(RoR2Content.Items.Knurl));
			}
		}
		private static void IL_RecalculateStats(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", IL_ItemName),
				x => ILPatternMatchingExt.MatchCallOrCallvirt<Inventory>(x, "GetItemCount"),
				x => ILPatternMatchingExt.MatchStloc(x, IL_Location)
			);
			ilcursor.Index -= IL_LocationOffset;
			ilcursor.RemoveRange(5);
		}
	}
}
