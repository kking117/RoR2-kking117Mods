using System;
using RoR2;
using RoR2.Orbs;
using R2API;
using EntityStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using FlatItemBuff.Utils;

namespace FlatItemBuff.Items
{
    public class SquidPolyp
    {
		//"RoR2/Base/Squid/cscSquidTurret.asset"
		public static SpawnCard SquidTurret_SpawnCard = Addressables.LoadAssetAsync<SpawnCard>("1d32d31aef1bac2499de2ec2312c6e9c").WaitForCompletion();
		//"RoR2/Base/Squid/SquidTurretBody.prefab"
		public static GameObject SquidTurret_Body = Addressables.LoadAssetAsync<GameObject>("5290eaf612d386740ac26f289e06b62f").WaitForCompletion();
		public static DeployableSlot SquidTurret_DeployableSlot;
		public DeployableAPI.GetDeployableSameSlotLimit SquidTurret_DeployableLimit;

		private const string LogName = "Squid Polyp";
		internal static bool Enable = false;
		internal static bool ApplyTar = true;
		internal static int BaseDuration = 25;
		internal static int StackDuration = 5;
		internal static int StackHealth = 2;
		internal static int MaxTurrets = 8;
        public SquidPolyp()
        {
			if (!Enable)
            {
				return;
            }
			MainPlugin.ModLogger.LogInfo(LogName);
			ClampConfig();
			UpdateText();
			if (MaxTurrets > 0)
            {
				CreateDeployableSlot();
			}
			UpdateBodyMaster();
			Hooks();
        }
		private void ClampConfig()
		{
			BaseDuration = Math.Max(0, BaseDuration);
			StackDuration = Math.Max(0, StackDuration);
			StackHealth = Math.Max(0, StackHealth);
			MaxTurrets = Math.Max(0, MaxTurrets);
		}
		private void CreateDeployableSlot()
		{
			SquidTurret_DeployableSlot = DeployableAPI.RegisterDeployableSlot(new DeployableAPI.GetDeployableSameSlotLimit(GetSquidTurret_DeployableLimit));
		}
		private int GetSquidTurret_DeployableLimit(CharacterMaster self, int deployableMult)
		{
			return MaxTurrets;
		}
		private void UpdateBodyMaster()
        {
			CharacterBody body = SquidTurret_Body.GetComponent<CharacterBody>();
			if (body)
            {
				body.baseDamage = 5f;
			}
		}
		private void UpdateText()
        {
			MainPlugin.ModLogger.LogInfo("Updating Text");
			string desc = "";
			if (ApplyTar)
			{
				desc = "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style> applying <style=cIsDamage>tar</style>.";
			}
			else
			{
				desc = "Activating an interactable summons a <style=cIsDamage>Squid Turret</style> that attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>.";
			}
			if (StackDuration > 0)
			{
				desc += string.Format(" Lasts <style=cIsUtility>{0} <style=cStack>(+{1} per stack)</style> seconds</style>.", BaseDuration, StackDuration);
			}
			else
			{
				desc += string.Format(" Lasts <style=cIsUtility>{0} seconds</style>.", BaseDuration);
			}
			LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", desc);
		}
        private void Hooks()
        {
			MainPlugin.ModLogger.LogInfo("Applying IL");
			IL.RoR2.GlobalEventManager.OnInteractionBegin += new ILContext.Manipulator(IL_InteractBegin);
			On.RoR2.GlobalEventManager.OnInteractionBegin += OnInteraction;
			IL.EntityStates.Squid.SquidWeapon.FireSpine.FireOrbArrow += new ILContext.Manipulator(IL_OnFire);
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
			itemCount = Math.Max(0, itemCount - 1);
			DirectorPlacementRule placementRule = new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				minDistance = 5f,
				maxDistance = 25f,
				position = position
			};
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(SquidTurret_SpawnCard, placementRule, RoR2Application.rng);
			directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Player);
			directorSpawnRequest.summonerBodyObject = summoner.gameObject;
			if (MaxTurrets > 0)
            {
				directorSpawnRequest.ignoreTeamMemberLimit = true;
			}
			DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
			directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
			{
				if (!result.success)
				{
					return;
				}
				CharacterMaster master = result.spawnedInstance.GetComponent<CharacterMaster>();
				int lifeTime = BaseDuration + (itemCount * StackDuration);
				master.inventory.GiveItem(RoR2Content.Items.HealthDecay, lifeTime);
				master.inventory.GiveItem(RoR2Content.Items.BoostHp, StackHealth * itemCount);
				master.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * itemCount);
				if (MaxTurrets > 0)
                {
					CharacterMaster ownerMaster = summoner.master;
					if (ownerMaster)
                    {
						int killCount = (ownerMaster.GetDeployableCount(SquidTurret_DeployableSlot) + 1) - ownerMaster.GetDeployableSameSlotLimit(SquidTurret_DeployableSlot);
						if (killCount > 0)
						{
							Helpers.KillDeployables(ownerMaster, SquidTurret_DeployableSlot, killCount);
						}
					}
					Deployable deployable = result.spawnedInstance.AddComponent<Deployable>();
					ownerMaster.AddDeployable(deployable, SquidTurret_DeployableSlot);
				}
			}));
			DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
		}
		private bool CanProcFromInteraction(IInteractable interactable, GameObject interactableObject)
        {
			//InteractableIsPermittedForSpawn(MonoBehaviour interactableAsMonoBehaviour) from ILSpy
			MonoBehaviour monoBehaviour = (MonoBehaviour)interactable;
			if (!monoBehaviour)
            {
				return false;
            }
			InteractionProcFilter procfilter = interactableObject.GetComponent<InteractionProcFilter>();
			if (procfilter)
			{
				return procfilter.shouldAllowOnInteractionBeginProc;
			}
			if (monoBehaviour.GetComponent<DelusionChestController>())
            {
				if (monoBehaviour.GetComponent<PickupPickerController>().enabled)
				{
					return false;
				}
				return true;
			}
			if (monoBehaviour.GetComponent<GenericPickupController>())
            {
				return false;
            }
			if (monoBehaviour.GetComponent<VehicleSeat>())
			{
				return false;
			}
			if (monoBehaviour.GetComponent<NetworkUIPromptController>())
			{
				return false;
			}
			return true;
		}
		private void IL_InteractBegin(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ilcursor.TryGotoNext(
				x => x.MatchLdloc(5),
				x => x.MatchLdloc(4),
				x => x.MatchLdsfld(typeof(RoR2Content.Items), "Squid")
			))
			{
				ilcursor.Index += 4;
				ilcursor.Emit(OpCodes.Ldc_I4_0);
				ilcursor.Emit(OpCodes.Mul);
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_InteractBegin - Hook failed");
			}
		}
		private void IL_OnFire(ILContext il)
		{
			ILCursor ilcursor = new ILCursor(il);
			if (ApplyTar)
			{
				if (ilcursor.TryGotoNext(
					x => x.MatchStloc(3)
				))
				{
					ilcursor.Index -= 1;
					ilcursor.Remove();
					ilcursor.EmitDelegate<Func<SquidOrb>>(() =>
					{
						SquidOrb orb = new SquidOrb();
						orb.damageType = DamageType.ClayGoo;
						return orb;
					});
				}
				else
				{
					UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName + " - IL_OnFire A - Hook failed");
				}
			}
			if (ilcursor.TryGotoNext(
				x => x.MatchStfld(typeof(SquidOrb), "forceScalar")
			))
			{
				ilcursor.Emit(OpCodes.Ldarg, 0);
				ilcursor.EmitDelegate<Func<float, EntityState, float>>((force, stateuser) =>
				{
					float minForce = force * 0.6f;
					float atkSpeed = 1f + Math.Max(0f, stateuser.characterBody.attackSpeed - 1f) * 0.2f;
					return Math.Max(minForce, force / atkSpeed);
				});
			}
			else
			{
				UnityEngine.Debug.LogError(MainPlugin.MODNAME + ": " + LogName  + " - IL_OnFire B - Hook failed");
			}
		}
	}
}
