using RoR2;

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
