using RoR2;
using UnityEngine;

namespace FlatItemBuff.Components
{
    public class DefenseNucleusShield : MonoBehaviour
    {
        public NetworkedBodyAttachment shieldObject;
        public float duration;
        private void Awake()
        {
            CharacterBody body = GetComponent<CharacterBody>();
            if (!body)
            {
                Destroy(this);
            }
            shieldObject = Instantiate<GameObject>(Items.DefenseNucleus_Shared.ShieldPrefab).GetComponent<NetworkedBodyAttachment>();
            if (shieldObject)
            {
                shieldObject.AttachToGameObjectAndSpawn(body.gameObject, null);
                duration = Items.DefenseNucleus_Rework.ShieldBaseDuration;
            }
            else
            {
                Destroy(this);
            }
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
