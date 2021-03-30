using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using RoR2;
using EnigmaticThunder.Modules;
using UnityEngine;

namespace ArtifactOfGrief
{
    [BepInPlugin("com.kking117.ArtifactOfGrief", "ArtifactOfGrief", "1.0.0")]
    [BepInDependency("com.EnigmaDev.EnigmaticThunder", "0.1.0")]
    public class ArtifactOfGrief : BaseUnityPlugin
    {
        private RoR2.ArtifactDef Grief = ScriptableObject.CreateInstance<RoR2.ArtifactDef>();
        private RoR2.BuffDef Thief = ScriptableObject.CreateInstance<RoR2.BuffDef>();
        public static ConfigEntry<int> MinDropAmount;
        public static ConfigEntry<int> MaxDropAmount;
        public static ConfigEntry<float> DropThresh;
        public static ConfigEntry<float> MinDamageForce;
        public static ConfigEntry<float> MaxDamageForce;
        public static ConfigEntry<float> MoveForce;
        public static ConfigEntry<bool> TriggerDOT;
        public static ConfigEntry<bool> TriggerOther;
        public static ConfigEntry<bool> TriggerSelf;
        public static ConfigEntry<bool> TriggerFF;
        public static ConfigEntry<bool> OrderTier;
        public static ConfigEntry<bool> IncludeBoss;
        public static ConfigEntry<bool> IncludeLunar;
        public static ConfigEntry<bool> EnemyCanPickup;
        public void Awake()
        {
            ReadConfig();
            SetupArtifact();
            
            //Enemy Item Pickup Related Hooks
            On.RoR2.CharacterMaster.OnBodyDeath += (orig, master, body) =>
            {
                bool couldrespawn = false;
                if (master.inventory.GetItemCount(RoR2Content.Items.ExtraLife) > 0)
                {
                    couldrespawn = true;
                }
                orig(master, body);
                if (!couldrespawn)
                {
                    DropStolenInventory(body);
                }
            };
            On.RoR2.GenericPickupController.BodyHasPickupPermission += (orig, body) =>
            {
                if (body.masterObject)
                {
                    if (body.inventory)
                    {
                        if (RunArtifactManager.instance.IsArtifactEnabled(Grief.artifactIndex) && EnemyCanPickup.Value)
                        {
                            MinionOwnership minionowner = body.master.minionOwnership;
                            if (!minionowner || !minionowner.ownerMaster)
                            {
                                return true;
                            }
                        }
                        else if (body.masterObject.GetComponent<PlayerCharacterMasterController>())
                        {
                            TeamComponent component = body.GetComponent<TeamComponent>();
                            if (component && component.teamIndex == TeamIndex.Player)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            };
            On.RoR2.GenericPickupController.AttemptGrant += (orig, gpc, body) =>
            {
                orig(gpc, body);
                if (RunArtifactManager.instance.IsArtifactEnabled(Grief.artifactIndex) && EnemyCanPickup.Value)
                {
                    TeamComponent component = body.GetComponent<TeamComponent>();
                    if (component && component.teamIndex != TeamIndex.Player)
                    {
                        Inventory inventory = body.inventory;
                        if (inventory)
                        {
                            PickupDef pickupDef = PickupCatalog.GetPickupDef(gpc.pickupIndex);
                            if (pickupDef.itemIndex != ItemIndex.None)
                            {
                                gpc.gameObject.SetActive(false);
                                inventory.GiveItem(pickupDef.itemIndex, 1);
                                Chat.AddPickupMessage(body, ((pickupDef != null) ? pickupDef.nameToken : null) ?? PickupCatalog.invalidPickupToken, (pickupDef != null) ? pickupDef.baseColor : Color.black, 1);
                                if (body)
                                {
                                    Util.PlaySound("Play_UI_item_pickup", body.gameObject);
                                }
                                AddToStolenInventory(body, body.master.gameObject, pickupDef.itemIndex, 1);
                                UnityEngine.Object.Destroy(gpc.gameObject);
                            }
                        }
                    }
                }
            };
            //Setup Grief from Damage Report
            On.RoR2.CharacterBody.OnTakeDamageServer += (orig, charbody, damageReport) =>
            {
                orig(charbody, damageReport);
                if (RunArtifactManager.instance.IsArtifactEnabled(Grief.artifactIndex))
                {
                    if (charbody.master)
                    {
                        CharacterMaster master = charbody.master;
                        if(master.playerCharacterMasterController)
                        {
                            SetupGriefFromDamageReport(master, damageReport);
                        }
                    }
                }
            };
        }
        //Stolen Inventory Functions
        void AddToStolenInventory(CharacterBody body, GameObject obj, ItemIndex item, int count)
        {
            body.AddBuff(Thief);
            StolenInventory inventory = obj.GetComponent<StolenInventory>();
            if (!inventory)
            {
                obj.AddComponent<StolenInventory>().amount[(int)item] += count;
            }
            else
            {
                inventory.amount[(int)item] += count;
            }
        }
        void DropStolenInventory(CharacterBody body)
        {
            GameObject obj = body.master.gameObject;
            StolenInventory inventory = obj.GetComponent<StolenInventory>();
            if (inventory)
            {
                for (int i = 0; i < inventory.amount.Length; i++)
                {
                    if (inventory.amount[i] > 0)
                    {
                        DropItem(body.master, (ItemIndex)i, inventory.amount[i], new Vector3(0f, 0f, 0f), false);
                        obj.AddComponent<StolenInventory>().amount[i] = 0;
                    }
                }
            }
        }
        static MethodInfo GetMethod(Type type, string methodName, BindingFlags flags)
        {
            var mi = type.GetMethod(methodName, flags);
            if (mi != null)
                return mi;
            if (type.BaseType != null)
                return GetMethod(type.BaseType, methodName, flags);
            return null;
        }
        public void SetupGriefFromDamageReport(CharacterMaster master, DamageReport report)
        {
            //ignore environment and invalid attackers
            if (!report.attackerBody)
            {
                if (!TriggerOther.Value)
                {
                    return;
                }
            }
            else
            {
                //ignore self harm
                if (report.attackerBody.master == master)
                {
                    if (!TriggerSelf.Value)
                    {
                        return;
                    }
                }
                //ignore friendly fire
                if (report.isFriendlyFire)
                {
                    if (!TriggerFF.Value)
                    {
                        return;
                    }
                }
            }
            //ignore dot damage
            if (report.dotType != DotController.DotIndex.None)
            {
                if (!TriggerDOT.Value)
                {
                    return;
                }
            }
            CharacterBody charbody = master.GetBody();
            HealthComponent hc = charbody.healthComponent;
            if (hc.combinedHealth > 0) //don't drop items if they're dead, there's a plugin that drops items on death anyway
            {
                float centdmg = report.damageDealt / hc.fullCombinedHealth;
                if (centdmg >= 0.0f)
                {
                    RandomDropItemFromGrief(master, centdmg, report.damageInfo.force);
                }
            }
        }

        public void RandomDropItemFromGrief(CharacterMaster master, float centdmg, Vector3 force)
        {
            //Gather all valid items the player can drop.
            //0 = index
            //1 = total amount;
            int[,] WhiteItems = new int[1500, 2];
            int[,] GreenItems = new int[300, 2];
            int[,] RedItems = new int[300, 2];
            int[,] YellowItems = new int[300, 2];
            int[,] LunarItems = new int[300, 2];
            int[] MaxCounts = new int[5];
            int TotalValidItems = 0;
            Inventory inventory = master.inventory;
            for(int i=0; i<RoR2.ItemCatalog.itemCount; i++)
            {
                if(inventory.GetItemCount((ItemIndex)i)>0)
                {
                    int amount = inventory.GetItemCount((ItemIndex)i);
                    ItemDef def = ItemCatalog.GetItemDef((ItemIndex)i);
                    if (OrderTier.Value)
                    {
                        switch (def.tier)
                        {
                            case ItemTier.Tier1:
                                WhiteItems[MaxCounts[0], 0] = i;
                                WhiteItems[MaxCounts[0], 1] = amount;
                                MaxCounts[0] += 1;
                                TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                                break;
                            case ItemTier.Tier2:
                                GreenItems[MaxCounts[1], 0] = i;
                                GreenItems[MaxCounts[1], 1] = amount;
                                MaxCounts[1] += 1;
                                TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                                break;
                            case ItemTier.Tier3:
                                RedItems[MaxCounts[2], 0] = i;
                                RedItems[MaxCounts[2], 1] = amount;
                                MaxCounts[2] += 1;
                                TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                                break;
                            case ItemTier.Boss:
                                if (IncludeBoss.Value)
                                {
                                    YellowItems[MaxCounts[3], 0] = i;
                                    YellowItems[MaxCounts[3], 1] = amount;
                                    MaxCounts[3] += 1;
                                    TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                                }
                                break;
                            case ItemTier.Lunar:
                                if (IncludeLunar.Value)
                                {
                                    LunarItems[MaxCounts[4], 0] = i;
                                    LunarItems[MaxCounts[4], 1] = amount;
                                    MaxCounts[4] += 1;
                                    TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                                }
                                break;
                        }
                    }
                    else
                    {
                        if(def.tier == ItemTier.Tier1 && def.tier == ItemTier.Tier2 && def.tier == ItemTier.Tier3)
                        {
                            WhiteItems[MaxCounts[0], 0] = i;
                            WhiteItems[MaxCounts[0], 1] = amount;
                            MaxCounts[0] += 1;
                            TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                        }
                        //very lazy
                        if (def.tier == ItemTier.Boss && IncludeBoss.Value)
                        {
                            WhiteItems[MaxCounts[0], 0] = i;
                            WhiteItems[MaxCounts[0], 1] = amount;
                            MaxCounts[0] += 1;
                            TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                        }
                        if (def.tier == ItemTier.Lunar && IncludeLunar.Value)
                        {
                            WhiteItems[MaxCounts[0], 0] = i;
                            WhiteItems[MaxCounts[0], 1] = amount;
                            MaxCounts[0] += 1;
                            TotalValidItems += inventory.GetItemCount((ItemIndex)i);
                        }
                    }
                }
            }
            if (TotalValidItems > MaxDropAmount.Value)
            {
                TotalValidItems = MaxDropAmount.Value;
            }
            if (MaxDropAmount.Value > 0 && TotalValidItems > MaxDropAmount.Value)
            {
                TotalValidItems = MaxDropAmount.Value;
            }
            if(centdmg>1.0)
            {
                centdmg = 1.0f;
            }
            force = ScaleVector(force, MinDamageForce.Value+((MaxDamageForce.Value-MinDamageForce.Value) * centdmg));
            //get how many items they will drop from the hit
            int ItemDrops = (int)(TotalValidItems * centdmg);
            if(MinDropAmount.Value > 0 && ItemDrops < MinDropAmount.Value && TotalValidItems > 0)
            {
                ItemDrops = 1;
            }
            if (ItemDrops>1 && DropThresh.Value > centdmg)
            {
                ItemDrops = 1;
            }
            //shuffle arrays to randomize drops
            if (ItemDrops > 0)
            {
                for (int i = 0; i < MaxCounts[0]; i++)
                {
                    if (WhiteItems[i, 0] > -1 && WhiteItems[i, 1] > 0)
                    {
                        int newslot = UnityEngine.Random.Range(0, MaxCounts[0]);
                        int[] oldslot = new int[2];
                        oldslot[0] = WhiteItems[i, 0];
                        oldslot[1] = WhiteItems[i, 1];
                        WhiteItems[i, 0] = WhiteItems[newslot, 0];
                        WhiteItems[i, 1] = WhiteItems[newslot, 1];
                        WhiteItems[newslot, 0] = oldslot[0];
                        WhiteItems[newslot, 1] = oldslot[1];
                    }
                }
                for (int i = 0; i < MaxCounts[1]; i++)
                {
                    if (GreenItems[i, 0] > -1 && GreenItems[i, 1] > 0)
                    {
                        int newslot = UnityEngine.Random.Range(0, MaxCounts[1]);
                        int[] oldslot = new int[2];
                        oldslot[0] = GreenItems[i, 0];
                        oldslot[1] = GreenItems[i, 1];
                        GreenItems[i, 0] = GreenItems[newslot, 0];
                        GreenItems[i, 1] = GreenItems[newslot, 1];
                        GreenItems[newslot, 0] = oldslot[0];
                        GreenItems[newslot, 1] = oldslot[1];
                    }
                }
                for (int i = 0; i < MaxCounts[2]; i++)
                {
                    if (RedItems[i, 0] > -1 && RedItems[i, 1] > 0)
                    {
                        int newslot = UnityEngine.Random.Range(0, MaxCounts[2]);
                        int[] oldslot = new int[2];
                        oldslot[0] = RedItems[i, 0];
                        oldslot[1] = RedItems[i, 1];
                        RedItems[i, 0] = RedItems[newslot, 0];
                        RedItems[i, 1] = RedItems[newslot, 1];
                        RedItems[newslot, 0] = oldslot[0];
                        RedItems[newslot, 1] = oldslot[1];
                    }
                }
                for (int i = 0; i < MaxCounts[3]; i++)
                {
                    if (YellowItems[i, 0] > -1 && YellowItems[i, 1] > 0)
                    {
                        int newslot = UnityEngine.Random.Range(0, MaxCounts[3]);
                        int[] oldslot = new int[2];
                        oldslot[0] = YellowItems[i, 0];
                        oldslot[1] = YellowItems[i, 1];
                        YellowItems[i, 0] = YellowItems[newslot, 0];
                        YellowItems[i, 1] = YellowItems[newslot, 1];
                        YellowItems[newslot, 0] = oldslot[0];
                        YellowItems[newslot, 1] = oldslot[1];
                    }
                }
                for (int i = 0; i < MaxCounts[4]; i++)
                {
                    if (LunarItems[i, 0] > -1 && LunarItems[i, 1] > 0)
                    {
                        int newslot = UnityEngine.Random.Range(0, MaxCounts[4]);
                        int[] oldslot = new int[2];
                        oldslot[0] = LunarItems[i, 0];
                        oldslot[1] = LunarItems[i, 1];
                        LunarItems[i, 0] = LunarItems[newslot, 0];
                        LunarItems[i, 1] = LunarItems[newslot, 1];
                        LunarItems[newslot, 0] = oldslot[0];
                        LunarItems[newslot, 1] = oldslot[1];
                    }
                }
                //drop items in order of tier
                if (ItemDrops > 0)
                {
                    for (int i = 0; i < MaxCounts[0]; i++)
                    {
                        if (WhiteItems[i, 0] > -1 && WhiteItems[i, 1] > 0)
                        {
                            int drops = WhiteItems[i, 1];
                            if (drops > ItemDrops)
                            {
                                drops = ItemDrops;
                            }
                            DropItem(master, (ItemIndex)WhiteItems[i, 0], drops, force, true);
                            ItemDrops -= drops;
                        }
                        if (ItemDrops < 1)
                        {
                            break;
                        }
                    }
                }
                if (ItemDrops > 0)
                {
                    for (int i = 0; i < MaxCounts[1]; i++)
                    {
                        if (GreenItems[i, 0] > -1 && GreenItems[i, 1] > 0)
                        {
                            int drops = GreenItems[i, 1];
                            if (drops > ItemDrops)
                            {
                                drops = ItemDrops;
                            }
                            DropItem(master, (ItemIndex)GreenItems[i, 0], drops, force, true);
                            ItemDrops -= drops;
                        }
                        if (ItemDrops < 1)
                        {
                            break;
                        }
                    }
                }
                if (ItemDrops > 0)
                {
                    for (int i = 0; i < MaxCounts[2]; i++)
                    {
                        if (RedItems[i, 0] > -1 && RedItems[i, 1] > 0)
                        {
                            int drops = RedItems[i, 1];
                            if (drops > ItemDrops)
                            {
                                drops = ItemDrops;
                            }
                            DropItem(master, (ItemIndex)RedItems[i, 0], drops, force, true);
                            ItemDrops -= drops;
                        }
                        if (ItemDrops < 1)
                        {
                            break;
                        }
                    }
                }
                if (ItemDrops > 0)
                {
                    for (int i = 0; i < MaxCounts[3]; i++)
                    {
                        if (YellowItems[i, 0] > -1 && YellowItems[i, 1] > 0)
                        {
                            int drops = YellowItems[i, 1];
                            if (drops > ItemDrops)
                            {
                                drops = ItemDrops;
                            }
                            DropItem(master, (ItemIndex)YellowItems[i, 0], drops, force, true);
                            ItemDrops -= drops;
                        }
                        if (ItemDrops < 1)
                        {
                            break;
                        }
                    }
                }
                if (ItemDrops > 0)
                {
                    for (int i = 0; i < MaxCounts[4]; i++)
                    {
                        if (LunarItems[i, 0] > -1 && LunarItems[i, 1] > 0)
                        {
                            int drops = LunarItems[i, 1];
                            if (drops > ItemDrops)
                            {
                                drops = ItemDrops;
                            }
                            DropItem(master, (ItemIndex)LunarItems[i, 0], drops, force, true);
                            ItemDrops -= drops;
                        }
                        if (ItemDrops < 1)
                        {
                            break;
                        }
                    }
                }
            }
        }
        public void SetupArtifact()
        {
            Grief.nameToken = "Artifact of Grief";
            string desc = "Drop items when hit.";
            if(EnemyCanPickup.Value)
            {
                desc += " Enemies can pick up items.";
            }
            Grief.descriptionToken = desc;
            Grief.smallIconDeselectedSprite = Loadouts.CreateSkinIcon(Color.gray, Color.gray, Color.gray, Color.gray);
            Grief.smallIconSelectedSprite = Loadouts.CreateSkinIcon(Color.white, Color.green, Color.red, Color.yellow);
            Artifacts.RegisterArtifact(Grief);

            //Thief.iconSprite = RoR2Content.Buffs.FullCrit.iconSprite;
            Thief.canStack = true;
            Thief.buffColor = Color.gray;
            Buffs.RegisterBuff(Thief);
        }

        void DropItem(CharacterMaster master, ItemIndex item, int amount, Vector3 force, bool drain)
        {
            if (item != ItemIndex.None)
            {
                CharacterBody body = master.GetBody();
                if (drain)
                {
                    master.inventory.RemoveItem(item, amount);
                }
                for (; amount > 0; amount--)
                {
                    Vector3 throwangle = new Vector3(1.0f, 1.0f, 1.0f);
                    throwangle = GetThrowAngle(throwangle);
                    throwangle *= 15.0f;
                    throwangle += force;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(item), body.inputBank.aimOrigin, throwangle);
                }
            }
        }

        Vector3 GetThrowAngle(Vector3 angle)
        {
            //Rotate the angle by a random degree
            float rotation = UnityEngine.Random.Range(0.0f, 360.0f);
            float x = angle[0];
            float y = angle[2];
            angle[0] = x * (float)Math.Cos(rotation) - y * (float)Math.Sin(rotation);
            angle[2] = x * (float)Math.Sin(rotation) + y * (float)Math.Cos(rotation);
            //Then normalize the angles
            angle[0] = angle[0] / 3.0f;
            angle[2] = angle[2] / 3.0f;
            angle[1] = 1.0f;
            return angle;
        }

        Vector3 ScaleVector(Vector3 vecta, float scale)
        {
            vecta = Vector3.Normalize(vecta);
            vecta[0] *= scale;
            vecta[1] *= scale;
            vecta[2] *= scale;
            return vecta;
        }
        public void ReadConfig()
        {
            MinDropAmount = Config.Bind<int>(new ConfigDefinition("ArtifactofGrief", "Min Drop Count"), 1, new ConfigDescription("The minimum amount of items you can lose from a single hit. (0 or less disables this.)", null, Array.Empty<object>()));
            MaxDropAmount = Config.Bind<int>(new ConfigDefinition("ArtifactofGrief", "Max Drop Count"), 30, new ConfigDescription("The maximum amount of items you can lose from a single hit. (0 or less disables this.)", null, Array.Empty<object>()));
            DropThresh = Config.Bind<float>(new ConfigDefinition("ArtifactofGrief", "Drop Threshold"), 0.05f, new ConfigDescription("The minimum percent of health to lose in a single hit to drop more than one item. (0.0 or less disables this.)", null, Array.Empty<object>()));

            OrderTier = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Order Tier"), true, new ConfigDescription("Items drop in order of tiers (White, Green, Red, Boss, Lunar) instead of randomly.", null, Array.Empty<object>()));

            IncludeBoss = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Allow Boss"), true, new ConfigDescription("Allows boss items to drop when enabled.", null, Array.Empty<object>()));
            IncludeLunar = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Allow Lunar"), false, new ConfigDescription("Allows lunar items to drop when enabled.", null, Array.Empty<object>()));

            MinDamageForce = Config.Bind<float>(new ConfigDefinition("ArtifactofGrief", "Min Damage Force"), 5.0f, new ConfigDescription("When dropping items, this is the minimum extra velocity they will be thrown based on the directional force of the hit.", null, Array.Empty<object>()));
            MaxDamageForce = Config.Bind<float>(new ConfigDefinition("ArtifactofGrief", "Max Damage Force"), 30.0f, new ConfigDescription("When dropping items, this is the maximum extra velocity they will be thrown based on the directional force of the hit. (This is how much force to apply at 100% health damage in a single hit.)", null, Array.Empty<object>()));

            TriggerDOT = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Trigger from DOT"), false, new ConfigDescription("Toggles if damage overtime should trigger item dropping.", null, Array.Empty<object>()));
            TriggerFF = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Trigger from Friendly Fire"), false, new ConfigDescription("Toggles if friendly fire should trigger item dropping.", null, Array.Empty<object>()));
            TriggerSelf = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Trigger from Self"), false, new ConfigDescription("Toggles if self-harm should trigger item dropping.", null, Array.Empty<object>()));
            TriggerOther = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Trigger from Other"), false, new ConfigDescription("Toggles if damage from environment and invalid attackers should trigger item dropping.", null, Array.Empty<object>()));

            EnemyCanPickup = Config.Bind<bool>(new ConfigDefinition("ArtifactofGrief", "Item Stealing"), true, new ConfigDescription("Enables any non-summoned character to grab items. (These stolen items will be dropped on their death)", null, Array.Empty<object>()));
        }
    }
    public class StolenInventory : MonoBehaviour
    {
        public int[] amount = new int[300];
    }
}
