using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
    public class Construct_CooldownManager : MonoBehaviour
    {
        public float duration = 7f;
        public int maxSummons = 4;
        private void FixedUpdate()
        {
            if (duration <= 0f)
            {
                RemoveSelf();
                return;
            }
            float timeRate = 1f;
            CharacterMaster master = GetComponent<CharacterMaster>();
            if (master)
            {
                if (master.GetDeployableCount(DeployableSlot.MinorConstructOnKill) < maxSummons)
                {
                    timeRate += 1f;
                }
            }
            else
            {
                RemoveSelf();
                return;
            }
            duration -= Time.fixedDeltaTime * timeRate;
        }
        private void RemoveSelf()
        {
            Destroy(this);
        }
    }
}
