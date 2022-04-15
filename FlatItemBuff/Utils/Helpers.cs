using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace FlatItemBuff.Utils
{
    internal class Helpers
    {
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
		public static void KillDeployableInRange(CharacterMaster owner, DeployableSlot slot, int killAmount, Vector3 origin, float searchDistance, bool isPriority)
		{
			if (owner)
			{
				if (owner.GetDeployableCount(slot) > 0)
				{
					List<CharacterMaster> HitList = new List<CharacterMaster>();
					BullseyeSearch search = new BullseyeSearch();
					search.viewer = null;
					search.teamMaskFilter = TeamMask.all;
					search.sortMode = BullseyeSearch.SortMode.Distance;
					search.maxDistanceFilter = searchDistance;
					search.searchOrigin = origin;
					search.searchDirection = origin;
					search.maxAngleFilter = 180f;
					search.filterByLoS = false;
					search.RefreshCandidates();
					foreach (HurtBox target in search.GetResults())
					{
						if (target && target.healthComponent)
						{
							if (target.healthComponent.body)
							{
								CharacterBody targetbody = target.healthComponent.body;
								if (targetbody.master)
								{
									CharacterMaster targetmaster = target.healthComponent.body.master;
									Deployable deployable = targetmaster.GetComponent<Deployable>();
									if (deployable)
									{
										if (deployable.ownerMaster == owner)
										{
											HitList.Add(targetmaster);
										}
									}
								}
							}
						}
					}

					for (int i = 0; i < HitList.Count && killAmount > 0; i++)
					{
						CharacterMaster target = HitList[i];
						Deployable deployable = target.GetComponent<Deployable>();
						if (deployable)
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
											target.TrueKill();
											killAmount--;
											break;
										}
									}
								}
							}
						}
					}
					if(isPriority && killAmount > 0)
                    {
						KillDeployables(owner, slot, killAmount);
					}
				}
			}
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
