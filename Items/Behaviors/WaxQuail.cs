using System;
using RoR2;

namespace FlatItemBuff.Items.Behaviors
{
    public class WaxQuail : CharacterBody.ItemBehavior
    {
		private bool WasGrounded = false;
		private void FixedUpdate()
        {
			if (!body)
			{
				return;
			}
			CharacterMotor motor = body.characterMotor;
			if (motor)
            {
				if (motor.isGrounded != WasGrounded)
                {
					WasGrounded = motor.isGrounded;
					body.MarkAllStatsDirty();
				}
			}
        }
	}
}
