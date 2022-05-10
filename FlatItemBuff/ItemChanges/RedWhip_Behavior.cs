using System;
using RoR2;

namespace FlatItemBuff.ItemChanges
{
    public class RedWhip_Behavior : CharacterBody.ItemBehavior
    {
		private void FixedUpdate()
        {
			if (!body)
			{
				return;
			}
			if (body.HasBuff(RoR2Content.Buffs.WhipBoost.buffIndex))
            {
				SetBuffStackCount(0);
			}
			else
            {
				if(body.outOfCombatStopwatch >= RedWhip.BuildUpSecond * 1f)
                {
					float stopWatch = Math.Min(MainPlugin.General_OutOfCombatTime.Value, body.outOfCombatStopwatch);
					int stackCount = (int)Math.Ceiling(Math.Max(0f, stopWatch - RedWhip.BuildUpSecond) * 2);
					SetBuffStackCount(stackCount);
				}
				else
                {
					SetBuffStackCount(0);
				}
            }
        }
		private void OnDisable()
		{
			SetBuffStackCount(0);
		}
		private void SetBuffStackCount(int stackCount)
        {
			stackCount = Math.Max(stackCount, 0);
			while(body.GetBuffCount(RedWhip.WhipBuildBuff.buffIndex) > stackCount)
            {
				body.RemoveBuff(RedWhip.WhipBuildBuff.buffIndex);
			}
			while (body.GetBuffCount(RedWhip.WhipBuildBuff.buffIndex) < stackCount)
			{
				body.AddBuff(RedWhip.WhipBuildBuff.buffIndex);
			}
		}
	}
}
