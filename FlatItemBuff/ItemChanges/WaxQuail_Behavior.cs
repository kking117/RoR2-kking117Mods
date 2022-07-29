using System;
using RoR2;

namespace FlatItemBuff.ItemChanges
{
    public class WaxQuail_Behavior : CharacterBody.ItemBehavior
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
