using System;
using RoR2;
using RoR2.Skills;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using FlatItemBuff.Components;
using FlatItemBuff.Utils;

namespace FlatItemBuff.ItemChanges
{
    public class SquidPolyp
    {
        public static void EnableChanges()
        {
			MainPlugin.ModLogger.LogInfo("Changing Squid Polyp");
			if (MainPlugin.Squid_ClayHit.Value)
            {
				ModifySquidSkill();
			}
			UpdateText();
			Hooks();
        }
		private static void UpdateText()
        {
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>";
			if (MainPlugin.Squid_ClayHit.Value)
			{
				desc += " applying <style=cIsDamage>tar</style>.";
			}
			else
			{
				desc += ".";
			}
			if (MainPlugin.Squid_StackLife.Value > 0)
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> <style=cStack>(+" + MainPlugin.Squid_StackLife.Value + " per stack)</style> seconds.";
			}
			else
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> seconds.";
			}
			LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", desc);
		}
        private static void Hooks()
        {
            On.RoR2.CharacterMaster.OnBodyStart += OnBodyStart;
        }
		public static void OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
		{
			orig(self, body);
			//So as you can see this is a fairly unreliable and generally a bad way to do this
			//But the IL for the interaction code is a complete mess and I can't even get the stack count from there
			if (!NetworkServer.active)
            {
				return;
            }
			if (IsSquidPolyp(body))
            {
				MinionOwnership minionowner = self.minionOwnership;
				if (minionowner)
				{
					CharacterMaster owner = Helpers.GetOwner(minionowner);
					if (owner && owner != self)
					{
						int stacks = self.inventory.GetItemCount(RoR2Content.Items.BoostAttackSpeed);
						int decay = (int)Math.Floor(30 + (stacks * MainPlugin.Squid_StackLife.Value * 0.1f));
						self.inventory.RemoveItem(RoR2Content.Items.HealthDecay, self.inventory.GetItemCount(RoR2Content.Items.HealthDecay));
						self.inventory.GiveItem(RoR2Content.Items.HealthDecay, decay);
						body.baseArmor = MainPlugin.Squid_Armor.Value * stacks;
						if (MainPlugin.Squid_InactiveDecay.Value > 0f)
						{
							DisableHealManager component = body.gameObject.AddComponent<DisableHealManager>();
							if (component)
                            {
								component.MaxLifeTime = MainPlugin.Squid_InactiveDecay.Value;
							}
						}
					}
				}
			}
		}
		private static bool IsSquidPolyp(CharacterBody self)
        {
			if (self.bodyIndex != BodyCatalog.FindBodyIndex("SquidTurretBody"))
			{
				return false;
			}
			if (self.inventory)
            {
				if (self.inventory.GetItemCount(RoR2Content.Items.Ghost) == 0)
                {
					if (self.inventory.GetItemCount(RoR2Content.Items.HealthDecay) == 30)
                    {
						return true;
                    }
				}
			}
			return false;
        }
		private static void ModifySquidSkill()
		{
			MainPlugin.ModLogger.LogInfo("Altering Squid Skill");
			SkillDef skillDef = LegacyResourcesAPI.Load<SkillDef>("skilldefs/squidturretbody/squidturretbodyturret");
			if (skillDef)
            {
				skillDef.activationState = new SerializableEntityStateType(typeof(States.SquidFire));
			}
			Modules.States.RegisterState(typeof(States.SquidFire));
		}
	}
}
