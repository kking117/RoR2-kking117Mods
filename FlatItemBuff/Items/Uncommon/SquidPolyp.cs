using System;
using RoR2;
using RoR2.Skills;
using R2API;
using EntityStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using FlatItemBuff.Components;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace FlatItemBuff.Items
{
    public class SquidPolyp
    {
		internal static bool Enable = true;
		internal static bool ApplyTar = true;
		internal static float RemoveDelay = 10f;
		internal static int StackDuration = 3;
		internal static float StackArmor = 10f;
        public SquidPolyp()
        {
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo("Changing Squid Polyp");
			ClampConfig();
			UpdateText();
			if (ApplyTar)
			{
				ModifySquidSkill();
			}
			Hooks();
        }
		private void ClampConfig()
		{
			RemoveDelay = Math.Max(0f, RemoveDelay);
			StackDuration = Math.Max(0, StackDuration);
			StackArmor = Math.Max(0f, StackArmor);
		}
		private void UpdateText()
        {
			MainPlugin.ModLogger.LogInfo("Updating item text");
			string desc = "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>";
			if (ApplyTar)
			{
				desc += " applying <style=cIsDamage>tar</style>.";
			}
			else
			{
				desc += ".";
			}
			if (StackDuration > 0)
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> <style=cStack>(+" + StackDuration + " per stack)</style> seconds.";
			}
			else
			{
				desc = desc + " Lasts <style=cIsUtility>30</style> seconds.";
			}
			LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", desc);
		}
        private void Hooks()
        {
			MainPlugin.ModLogger.LogInfo("Applying IL modifications");
			IL.RoR2.GlobalEventManager.OnInteractionBegin += new ILContext.Manipulator(IL_InteractBegin);
			On.RoR2.GlobalEventManager.OnInteractionBegin += OnInteraction;
		}
		private void OnInteraction(On.RoR2.GlobalEventManager.orig_OnInteractionBegin orig, GlobalEventManager self, Interactor interactor, IInteractable interactable, GameObject interactableObject)
		{
			orig(self, interactor, interactable, interactableObject);
			if(CanProcFromInteraction(interactable, interactableObject))
            {
				CharacterBody interactorBody = interactor.GetComponent<CharacterBody>();
				if (interactorBody)
				{
					Inventory inventory = interactorBody.inventory;
					if (inventory)
					{
						int itemCount = inventory.GetItemCount(RoR2Content.Items.Squid);
						if (itemCount > 0)
						{
							TrySpawnSquidPog(interactorBody, itemCount, interactableObject.transform.position);
						}
					}
				}
			}
		}
		private void TrySpawnSquidPog(CharacterBody summoner, int itemCount, Vector3 position)
        {
			if (itemCount > 0)
			{
				int stacks = Math.Max(0, itemCount - 1);
				SpawnCard spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscSquidTurret");
				DirectorPlacementRule placementRule = new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.Approximate,
					minDistance = 5f,
					maxDistance = 25f,
					position = position
				};
				DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
				directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Player);
				directorSpawnRequest.summonerBodyObject = summoner.gameObject;
				DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
				directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
				{
					if (!result.success)
					{
						return;
					}
					CharacterMaster master = result.spawnedInstance.GetComponent<CharacterMaster>();
					if (RemoveDelay > 0f)
					{
						InactiveManager component = master.gameObject.AddComponent<InactiveManager>();
						if (component)
                        {
							component.MaxLifeTime = RemoveDelay;
						}
					}
					master.inventory.GiveItem(RoR2Content.Items.HealthDecay, 30 + (stacks * StackDuration));
					master.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * stacks);
					CharacterBody body = master.GetBody();
					if (body)
					{
						body.baseArmor += StackArmor * stacks;
					}
				}));
				DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
			}
		}
		private bool CanProcFromInteraction(IInteractable interactable, GameObject interactableObject)
        {
			//DnSpy makes the interaction section almost unreadable for me
			//So I just used TheMysticSword-MysticsRisky2Utils for reference.
			MonoBehaviour monoBehaviour = (MonoBehaviour)interactable;
			if (!monoBehaviour.GetComponent<GenericPickupController>() && !monoBehaviour.GetComponent<VehicleSeat>() && !monoBehaviour.GetComponent<NetworkUIPromptController>())
			{
				InteractionProcFilter procfilter = interactableObject.GetComponent<InteractionProcFilter>();
				if (procfilter)
				{
					return procfilter.shouldAllowOnInteractionBeginProc;
				}
				return true;
			}
			return false;
		}
		private void ModifySquidSkill()
		{
			MainPlugin.ModLogger.LogInfo("Altering Squid Skill");
			SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Squid/SquidTurretBodyTurret.asset").WaitForCompletion();
			if (skillDef)
			{
				skillDef.activationState = new SerializableEntityStateType(typeof(States.SquidFire));
			}
			Modules.States.RegisterState(typeof(States.SquidFire));
		}
		private void IL_InteractBegin(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			ilcursor.GotoNext(
				x => ILPatternMatchingExt.MatchLdloc(x, 5),
				x => ILPatternMatchingExt.MatchLdloc(x, 4),
				x => ILPatternMatchingExt.MatchLdsfld(x, "RoR2.RoR2Content/Items", "Squid")
			);
			if(ilcursor.Index > 0)
            {
				ilcursor.Index += 4;
				//Giga brain
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
		}
	}
}
