using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace FlatItemBuff.Components
{
    public class DefenseNucleusShield : MonoBehaviour
    {
        public NetworkedBodyAttachment shieldObject;
        public float duration;
        private void Awake()
        {
            CharacterBody body = GetComponent<CharacterBody>();
            shieldObject = Instantiate<GameObject>(Items.DefenseNucleus_Shared.ShieldPrefab).GetComponent<NetworkedBodyAttachment>();
            shieldObject.AttachToGameObjectAndSpawn(body.gameObject, null);
            duration = MainPlugin.NucleusRework_ShieldBaseDuration.Value;
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
            if (shieldObject && shieldObject.gameObject)
            {
                Destroy(shieldObject.gameObject);
            }
            Destroy(this);
        }
    }
}
