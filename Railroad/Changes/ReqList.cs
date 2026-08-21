using System;
using System.Linq;
using System.Collections.Generic;
using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Railroad.Changes
{

    public class ReqAllowData
    {
        internal int StageNumber = -1;
        internal bool UseStageOrder = false;
        internal ConfigPortalType PortalType = ConfigPortalType.NoPortal;
        internal PortalProgReqTags ReqTags = PortalProgReqTags.None;
    }
    public class ReqList
    {
        public ReqList()
        {

        }
        internal static bool IsStageNumberOrOrder(string inputstring)
        {
            if (inputstring.Contains("SO"))
            {
                return true;
            }
            if (inputstring.Contains("SN"))
            {
                return true;
            }
            int stageNum = 0;
            if (Int32.TryParse(inputstring, out stageNum))
            {
                return true;
            }
            return false;
        }

        internal static bool PassesReqDataList(List<ReqAllowData> allowDataList, int stageNumber, SceneDef sceneDef)
        {
            if (allowDataList != null)
            {
                for(int i = 0; i < allowDataList.Count; i++)
                {
                    if (allowDataList[i].StageNumber > -1)
                    {
                        if (allowDataList[i].UseStageOrder)
                        {
                            if (allowDataList[i].StageNumber == 0 || allowDataList[i].StageNumber == sceneDef.stageOrder)
                            {
                                if (PortalUtility.PassesPortalTags(allowDataList[i].ReqTags))
                                {
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            if (allowDataList[i].StageNumber == 0 || allowDataList[i].StageNumber == stageNumber)
                            {
                                if (PortalUtility.PassesPortalTags(allowDataList[i].ReqTags))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        internal static List<ReqAllowData> ReadStageNumberInput(string portalList, string errorCode = "NONE")
        {
            //Used for Celestial Orb and Primordial Teleporter.
            //Interprets a string and creates a list of ReqAllowData.
            //Designed to distinguish StageOrder vs StageNumber and take Portal Tags.
            List<ReqAllowData> returnList = new List<ReqAllowData>();
            if (portalList.Length > 0)
            {
                string[] items = portalList.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    string portalType = items[i].Trim();
                    string[] portalTags = null;
                    if (portalType.Contains(";"))
                    {
                        portalTags = portalType.Split(';');
                        portalType = portalTags[0];
                    }
                    if (IsStageNumberOrOrder(portalType))
                    {
                        ReqAllowData thisReqData = new ReqAllowData();
                        int stageNum = -1;
                        if (Int32.TryParse(portalType, out stageNum))
                        {
                            thisReqData.StageNumber = stageNum;
                        }
                        else
                        {
                            if (portalType.Contains("SO"))
                            {
                                thisReqData.UseStageOrder = true;
                            }
                            string portalNum = portalType.Replace("SO", "");
                            portalNum = portalNum.Replace("SN", "");
                            if (Int32.TryParse(portalNum, out stageNum))
                            {
                                thisReqData.StageNumber = stageNum;
                            }
                        }
                        //MainPlugin.ModLogger.LogWarning("[" + errorCode + "] Interpreted '" + portalType + "' as: " + stageNum);

                        //Here is where we check and gather its portal tags
                        PortalProgReqTags thisPortalTags = PortalProgReqTags.None;
                        if (portalTags != null && portalTags.Length > 1)
                        {
                            for (int j = 1; j < portalTags.Length; j++)
                            {
                                PortalProgReqTags enumPortalProgReqTag = PortalProgReqTags.None;
                                if (!Enum.TryParse(portalTags[j], out enumPortalProgReqTag))
                                {
                                    //check if the portal tag exists as option
                                    MainPlugin.ModLogger.LogWarning("[" + errorCode + ", " + portalType + "] Could not find Portal Tag: [" + portalTags[j] + "]");
                                }
                                else
                                {
                                    if ((thisPortalTags & enumPortalProgReqTag) != enumPortalProgReqTag)
                                    {
                                        thisPortalTags |= enumPortalProgReqTag;
                                        //MainPlugin.ModLogger.LogWarning("Registered PortalTag: " + enumPortalProgReqTag);
                                    }
                                    else
                                    {
                                        MainPlugin.ModLogger.LogWarning("[" + errorCode + ", " + portalType + "] Duplicate Portal Tag in config: [" + enumPortalProgReqTag + "]");
                                    }
                                }
                            }
                        }
                        thisReqData.ReqTags = thisPortalTags;
                        returnList.Add(thisReqData);
                        //MainPlugin.ModLogger.LogWarning("Registered PortalType: " + portalType);
                    }
                    else
                    {
                        MainPlugin.ModLogger.LogWarning("[" + errorCode + "|" + portalType + "] No stage number specified, skipping.");
                    }
                }
                if (returnList.Count < 1)
                {
                    returnList = null;
                }
            }
            return returnList;
        }
    }
}