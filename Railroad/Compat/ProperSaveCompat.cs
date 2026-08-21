using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RoR2;
using UnityEngine;

namespace Railroad.Compat
{
    internal static class ProperSaveCompat
    {
        internal static void Init()
        {
            ProperSave.SaveFile.OnGatherSaveData += SaveData_OnGather;
            ProperSave.Loading.OnLoadingEnded += SaveData_OnLoad;
        }
        public class RailRoadSaveData
        {
            public Changes.RunProgressFlags BossDefeatFlags = Changes.RunProgressFlags.None;
            public bool ArenaCleared = false;

            internal RailRoadSaveData()
            {
                //runs when saving
                //MainPlugin.ModLogger.LogInfo("SAVEDATA INIT");
                BossDefeatFlags = Railroad.Changes.RunFlags.SaveData_RunProgressFlags;
            }
            internal void Load()
            {
                //runs when loading
                //MainPlugin.ModLogger.LogInfo("SAVE LOADED");
                Railroad.Changes.RunFlags.SaveData_RunProgressFlags = BossDefeatFlags;
                //Railroad.Changes.RunFlags.LogRunData();
            }
        }

        private static void SaveData_OnGather(Dictionary<string, object> obj)
        {
            obj.Add("RailRoad_SaveData", new RailRoadSaveData());
        }

        private static void SaveData_OnLoad(ProperSave.SaveFile obj)
        {
            var data = obj.GetModdedData<RailRoadSaveData>("RailRoad_SaveData");
            if (data != null) data.Load();
        }
    }
}