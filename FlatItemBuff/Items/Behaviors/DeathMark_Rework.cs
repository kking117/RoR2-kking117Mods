using RoR2;

namespace FlatItemBuff.Items.Behaviors
{
    public class DeathMark_Rework : CharacterBody.ItemBehavior
    {
		private void OnDisable()
		{
			body.RemoveBuff(Items.DeathMark_Rework.DeathMarkReadyBuff);
			body.RemoveBuff(Items.DeathMark_Rework.DeathMarkCooldownBuff);
		}
		private void FixedUpdate()
        {
			if (!body)
			{
				return;
			}
			bool hasCooldown = body.HasBuff(Items.DeathMark_Rework.DeathMarkCooldownBuff);
			bool isReady = body.HasBuff(Items.DeathMark_Rework.DeathMarkReadyBuff);
			if (!hasCooldown && !isReady)
			{
				body.AddBuff(Items.DeathMark_Rework.DeathMarkReadyBuff);
			}
			if (hasCooldown && isReady)
			{
				body.RemoveBuff(Items.DeathMark_Rework.DeathMarkReadyBuff);
			}
		}
	}
}
