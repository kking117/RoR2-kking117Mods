using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using R2API;
using UnityEngine;

namespace FlatItemBuff.Utils
{
    internal class Helpers
    {
		public static bool InflictorHasModdedDamageType(GameObject inflictor, DamageAPI.ModdedDamageType moddedDamageType)
		{
			if (inflictor)
			{
				DamageAPI.ModdedDamageTypeHolderComponent moddedDamageTypeHolder = inflictor.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
				if (moddedDamageTypeHolder)
				{
					return moddedDamageTypeHolder.Has(moddedDamageType);
				}
			}
			return false;
		}
		public static void AddOrExtendBuff(CharacterBody body, BuffDef buffDef, float duration)
		{
			BuffIndex buffIndex = buffDef.buffIndex;
			for (int i = 0; i < body.timedBuffs.Count; i++)
			{
				if (body.timedBuffs[i].buffIndex == buffIndex)
				{
					duration += body.timedBuffs[i].timer;
					break;
				}
			}
			body.ClearTimedBuffs(buffIndex);
			body.AddTimedBuff(buffDef, duration);
		}
		public static void Add_ExtendBuffDuration(CharacterBody body, BuffDef buffDef, float addDuration)
		{
			BuffIndex buffIndex = buffDef.buffIndex;
			List<float> BuffDuration = new List<float>();
			for (int i = 0; i < body.timedBuffs.Count; i++)
			{
				if (body.timedBuffs[i].buffIndex == buffIndex)
				{
					BuffDuration.Add(body.timedBuffs[i].timer + addDuration);
				}
			}
			body.ClearTimedBuffs(buffIndex);
			for (int i = 0; i < BuffDuration.Count; i++)
			{
				body.AddTimedBuff(buffDef, BuffDuration[i]);
			}
		}
		public static float GetBuffDuration(CharacterBody body, BuffDef buffDef)
        {
			List<CharacterBody.TimedBuff> buffList = body.timedBuffs;
			for (int i = 0; i < buffList.Count; i++)
			{
				if (buffList[i].buffIndex == buffDef.buffIndex)
                {
					return buffList[i].timer;
                }
			}
			return 0f;
        }
		public static float HyperbolicResult(int itemCount, float baseBonus, float stackBonus, int hardCap)
		{
			float bonus = baseBonus + (stackBonus * (itemCount - 1));
			float result = hardCap - hardCap / (1 + bonus);
			return result;
		}
		public static CharacterMaster GetOwner(MinionOwnership minionowner)
		{
			CharacterMaster returnmaster = minionowner.ownerMaster;
			if (minionowner.ownerMaster)
			{
				minionowner = returnmaster.minionOwnership;
				if (minionowner.ownerMaster)
				{
					returnmaster = minionowner.ownerMaster;
				}
			}
			return returnmaster;
		}
		public static CharacterMaster GetTrueOwner(MinionOwnership minionowner)
		{
			CharacterMaster returnmaster = minionowner.ownerMaster;
			if (returnmaster)
			{
				do
				{
					minionowner = returnmaster.minionOwnership;
					if (minionowner.ownerMaster)
					{
						returnmaster = minionowner.ownerMaster;
					}
				} while (minionowner.ownerMaster);
			}
			return returnmaster;
		}
		public static CharacterMaster GetOwnerAsDeployable(CharacterMaster self, DeployableSlot slot)
		{
			Deployable deployable = self.GetComponent<Deployable>();
			if (deployable)
			{
				CharacterMaster owner = deployable.ownerMaster;
				if (owner)
				{
					if (owner.deployablesList != null)
					{
						for (int i = 0; i < owner.deployablesList.Count; i++)
						{
							if (slot == DeployableSlot.None || owner.deployablesList[i].slot == slot)
							{
								if (owner.deployablesList[i].deployable == deployable)
								{
									return owner;
								}
							}
						}
					}
				}
			}
			return null;
		}

		public static bool IsDeployableSlot(CharacterMaster self, DeployableSlot slot)
		{
			Deployable deployable = self.GetComponent<Deployable>();
			if (deployable)
			{
				CharacterMaster owner = deployable.ownerMaster;
				if (owner)
				{
					if (owner.deployablesList != null)
					{
						for (int i = 0; i < owner.deployablesList.Count; i++)
						{
							if (slot == DeployableSlot.None || owner.deployablesList[i].slot == slot)
							{
								if (owner.deployablesList[i].deployable == deployable)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}
		public static void KillCloseDeployables(CharacterMaster exception, TeamIndex team, DeployableSlot slot, Vector3 origin, float killRange)
        {
			List<Deployable> targetList = new List<Deployable>(GetDeployableListFromTeam(team, slot));
			for (int i = 0; i < targetList.Count; i++)
			{
				CharacterMaster master = targetList[i].GetComponent<CharacterMaster>();
				if (master && master != exception)
				{
					CharacterBody body = master.GetBody();
					if (body)
					{
						HealthComponent healthComponent = body.healthComponent;
						if (healthComponent && healthComponent.alive)
						{
							if (Vector3.Distance(body.transform.position, origin) <= killRange)
							{
								CharacterMaster owner = targetList[i].ownerMaster;
								if (owner)
								{
									owner.RemoveDeployable(targetList[i]);
								}
								master.TrueKill();
							}
						}
					}
				}
			}
		}
		internal static void RemoveBuffStacks(CharacterBody body, BuffDef buffDef, int amount)
        {
			if (amount < 1)
            {
				amount = body.GetBuffCount(buffDef);
            }
			else
            {
				amount = Math.Min(amount, body.GetBuffCount(buffDef));
			}
			if (amount > 0)
			{
				for (int i = 0; i < amount; i++)
				{
					body.RemoveBuff(buffDef);
				}
			}
        }
		private static List<Deployable> GetDeployableListFromTeam(TeamIndex team, DeployableSlot slot)
        {
			List<Deployable> deployableList = new List<Deployable>();
			ReadOnlyCollection<TeamComponent> teamList = TeamComponent.GetTeamMembers(team);
			for (int i = 0; i < teamList.Count; i++)
			{
				CharacterBody body = teamList[i].body;
				if (body)
				{
					CharacterMaster master = body.master;
					if (master)
					{
						if (master.deployablesList != null)
						{
							for (int z = 0; z < master.deployablesList.Count; z++)
							{
								if (master.deployablesList[z].deployable)
								{
									if (master.deployablesList[z].slot == slot)
									{
										deployableList.Add(master.deployablesList[z].deployable);
									}
								}
							}
						}
					}
				}
			}
			return deployableList;
		}
		public static void KillDeployables(CharacterMaster owner, DeployableSlot slot, int killAmount)
		{
			if (killAmount > 0 && owner)
			{
				if (owner.GetDeployableCount(slot) > 0)
				{
					for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count && killAmount > 0; i++)
					{
						CharacterMaster master = CharacterMaster.readOnlyInstancesList[i];
						if (master)
						{
							Deployable deployable = master.GetComponent<Deployable>();
							if (deployable)
							{
								if (deployable.ownerMaster == owner)
								{
									if (owner.deployablesList != null)
									{
										for (int z = 0; z < owner.deployablesList.Count; z++)
										{
											if (owner.deployablesList[z].deployable == deployable)
											{
												if (owner.deployablesList[z].slot == slot)
												{
													owner.RemoveDeployable(deployable);
													master.TrueKill();
													killAmount--;
													break;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
