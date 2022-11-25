using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Utils
{
    internal class Helpers
    {
		
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
		public static  CharacterMaster GetTrueOwner(MinionOwnership minionowner)
		{
			CharacterMaster returnmaster = minionowner.ownerMaster;
			if (minionowner.ownerMaster)
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
			if (owner)
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
