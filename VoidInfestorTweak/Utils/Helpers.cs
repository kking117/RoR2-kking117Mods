using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace TestTest.Utils
{
    internal class Helpers
    {
        public static bool CanInfest(CharacterBody attacker, CharacterMaster target)
        {
            CharacterBody body = target.GetBody();
            if (body && body.inventory)
            {
                if(body.healthComponent && body.healthComponent.alive)
                {
                    if (body.isPlayerControlled)
                    {
                        return false;
                    }
                    if (body.teamComponent.teamIndex == attacker.teamComponent.teamIndex)
                    {
                        return false;
                    }
                    if (body.inventory.currentEquipmentIndex == DLC1Content.Elites.Void.eliteEquipmentDef.equipmentIndex)
                    {
                        return false;
                    }
                    if (body.isBoss)
                    {
                        if(!MainPlugin.InfestFilter_Boss.Value)
                        {
                            return false;
                        }
                        if(attacker.teamComponent.teamIndex == TeamIndex.Player)
                        {
                            return false; //This could cause a softlock.
                        }
                    }
                    if (!MainPlugin.InfestFilter_Player.Value && body.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        return false;
                    }
                    if (!MainPlugin.InfestFilter_Void.Value && body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Void))
                    {
                        return false;
                    }
                    if (!MainPlugin.InfestFilter_Mechanical.Value && body.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                    {
                        return false;
                    }
                    if (!MainPlugin.InfestFilter_Self.Value && BodyCatalog.GetBodyName(body.bodyIndex) == "VoidInfestorBody")
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
