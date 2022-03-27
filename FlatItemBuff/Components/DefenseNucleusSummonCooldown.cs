using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
    public class DefenseNucleusSummonCooldown : MonoBehaviour
    {
        public float duration;
        private void Awake()
        {
            duration = 10f;
        }
        private void FixedUpdate()
        {
            if (!GetComponent<CharacterBody>() || duration <= 0f)
            {
                RemoveSelf();
            }
            duration -= Time.fixedDeltaTime;
        }
        private void RemoveSelf()
        {
            Destroy(this);
        }
    }
}
