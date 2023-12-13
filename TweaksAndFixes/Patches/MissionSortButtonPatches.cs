using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class MissionSortButtonPatches
    {
        private static TextMesh cargo;
        private static Dictionary<string, string> cargoNames = new Dictionary<string, string>()
        {
            {"very_big_crate", "Very Big Crate" },
            {"big_crate", "Big Crate" },
            {"crate", "Small Crate" },
            {"barrel_closed", "Barrel" },
        };

        public static void SortMissions(Mission[] missions)
        {
            MissionListUI.instance.DisplayMissions(missions);
        }

        [HarmonyPatch(typeof(MissionDetailsUI), "Start")]
        private static class MissionDetailsUIStartPatch
        {
            private static bool done;

            [HarmonyPrefix]
            public static void Prefix(MissionDetailsUI __instance, GameObject ___UI)
            {
                if (done) return;
                foreach (Transform item in ___UI.transform)
                {
                    foreach (Transform item2 in item)
                    {
                        TextMesh textMesh = item2.GetComponent<TextMesh>();
                        if (textMesh)
                        {
                            if (textMesh.text.Contains("Cargo"))
                            {
                                List<string> texts = new List<string>(textMesh.text.Split('\n'));
                                texts.Insert(1, "Size:");
                                textMesh.text = string.Join("\n", texts);
                            }
                            else if (item2.name.Contains("weight"))
                            {
                                item2.localPosition = new Vector3(item2.localPosition.x, item2.localPosition.y - 0.093f, item2.localPosition.z);
                            }
                            else if (item2.name.Contains("amount"))
                            {
                                GameObject go = GameObject.Instantiate(item2.gameObject);
                                go.transform.parent = item;
                                go.transform.localPosition = new Vector3(0.5f, item2.localPosition.y, item2.localPosition.z);
                                go.transform.localScale = item2.transform.localScale;
                                go.name = item2.name.Replace("amount", "size");
                                cargo = go.GetComponent<TextMesh>();
                                item2.localPosition = new Vector3(item2.localPosition.x, item2.localPosition.y - 0.106f, item2.localPosition.z);
                            }
                        }
                    }
                }
                foreach (Transform item in ___UI.transform.parent)
                {
                    if (item.name == "current mission buttons")
                    {
                        foreach (Transform item2 in item)
                        {
                            if (item2.name == "page buttons")
                            {
                                GameObject backButtons = item2.Find("mission button (back)").gameObject;
                                GameObject gameObject = GameObject.Instantiate(backButtons);
                                gameObject.transform.parent = item2;
                                gameObject.transform.localPosition = new Vector3(-0.654f, -0.336f, 0.022f);
                                gameObject.transform.localRotation = backButtons.transform.localRotation;
                                gameObject.transform.localScale = backButtons.transform.localScale;
                                gameObject.name = "sort button";
                                GameObject buttonGameobject = gameObject.GetComponentInChildren<GoPointerButton>().gameObject;
                                GameObject.Destroy(buttonGameobject.GetComponent<GoPointerButton>());
                                GPMissionSortButton button = buttonGameobject.AddComponent<GPMissionSortButton>();
                                button.text = gameObject.GetComponentInChildren<TextMesh>();
                                button.UpdateText();
                            }
                        }
                    }
                }
                done = true;
            }
        }

        [HarmonyPatch(typeof(MissionDetailsUI), "ClickButton")]
        private static class ClickButtonPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(MissionDetailsUI __instance, bool ___clickable, bool ___mapZoomedIn, Mission ___currentMission, GameObject ___UI)
            {
                if (!___clickable)
                {
                    return false;
                }
                if (___mapZoomedIn)
                {
                    __instance.InvokePrivateMethod("ZoomMap", false);
                    return false;
                }
                if (___currentMission.missionIndex == -1)
                {
                    PlayerMissions.AcceptMission(___currentMission);
                    MissionListUI.instance.DisplayMissions(___currentMission.originPort.GetMissions(MissionListUI.instance.GetPrivateField<int>("currentPage"), MissionListUI.instance.worldMissions));
                    ___UI.SetActive(false);
                    __instance.InvokePrivateMethod("UpdateTexts");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MissionDetailsUI), "UpdateTexts")]
        private static class MissionDetailsUIPatch
        {
            [HarmonyPostfix]
            public static void Postfix(TextMesh ___due, Mission ___currentMission)
            {
                if (___currentMission != null)
                {
                    string meshName = ___currentMission.goodPrefab.GetComponent<MeshFilter>().sharedMesh.name;
                    if (!cargoNames.TryGetValue(meshName, out string cargoName))
                    {
                        cargoName = meshName;
                    }
                    cargo.text = cargoName;
                }
            }
        }

        [HarmonyPatch(typeof(PortDude), "ActivateMissionListUI")]
        private static class ActivateMissionListUIPatch
        {
            [HarmonyPostfix]
            public static void Postfix(PortDude __instance)
            {
                GPMissionSortButton.currentPort = __instance.GetPort();
            }
        }

        [HarmonyPatch(typeof(PortDude), "DeactivateMissionListUI")]
        private static class DeactivateMissionListUIPatch
        {
            [HarmonyPostfix]
            public static void Postfix(PortDude __instance)
            {
                GPMissionSortButton.currentPort = null;
            }
        }

        [HarmonyPatch(typeof(Port), "Start")]
        private static class PortStartPatch
        {
            [HarmonyPostfix]
            public static void Postfix(Port __instance)
            {
                __instance.gameObject.AddComponent<MissionStoring>();
            }
        }

        [HarmonyPatch(typeof(IslandMissionOffice), "GenerateMissions")]
        private static class GenerateMissionsPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(IslandMissionOffice __instance, ref Mission[] __result, int ___maxGoodsPerMission, int[] ___goodsInWarehouse, IslandMarket ___market, bool world, int page, ref Port[] ___destinationPorts)
            {
                var array = new Mission[5];
                int num = page * array.Length;
                List<Mission> list = new List<Mission>();
                foreach (Port port in ___destinationPorts)
                {
                    if (___market.knownPrices[port.portIndex] != null && ___market.knownPrices[port.portIndex].approved)
                    {
                        int num2 = ___maxGoodsPerMission;
                        float num3 = DebugMarketTracker.instance.missionProfitShareLocal;
                        float distance = Mission.GetDistance(___market.GetPort(), port);
                        if(Mission.UseOceanMapFor(___market.GetPort(), port))
                        {
                            num2 *= 2;
                            num3 = DebugMarketTracker.instance.missionProfitShareWorld;
                        }
                        num2 = Mathf.Min(PlayerReputation.GetMaxMissionGoods(___market.GetPortRegion()), num2);
                        if ((distance < 200f || world) && (distance >= 200f || !world))
                        {
                            for (int j = 1; j < ___market.currentSupply.Length; j++)
                            {
                                int num4 = 0;
                                for (int k = 0; k < ___goodsInWarehouse.Length; k++)
                                {
                                    if (___goodsInWarehouse[k] == j && num4 < num2)
                                    {
                                        num4++;
                                    }
                                }
                                if (num4 > 0)
                                {
                                    float num5 = (float)___market.GetGoodPrice(j) * 1f;
                                    float num6 = ((float)___market.knownPrices[port.portIndex].sellPrices[j] - num5) * (float)num4;
                                    float num7 = num6 * num3;
                                    float num8 = distance * DebugMarketTracker.instance.missionDistanceFee;
                                    float num9 = (num7 + num8) * DebugMarketTracker.instance.missionFinalMult;
                                    bool flag = true;
                                    if (num8 > num6)
                                    {
                                        flag = false;
                                    }
                                    if (PrefabsDirectory.instance.GetGood(j).GetComponent<Good>().requiredRepLevel > PlayerReputation.GetRepLevel(___market.GetPort().region))
                                    {
                                        flag = false;
                                    }
                                    if (Mission.GetDistance(___market.GetPort(), port) > PlayerReputation.GetMaxDistance(___market.GetPort().region))
                                    {
                                        flag = false;
                                    }
                                    GameObject gameObject = null;
                                    if (flag)
                                    {
                                        gameObject = PrefabsDirectory.instance.GetGood(j).gameObject;
                                        if (__instance.InvokePrivateMethod<bool>("PlayerAlreadyHasMission", ___market.GetPort(), port, gameObject))
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (flag)
                                    {
                                        int num10 = Mathf.RoundToInt(num9) / num4 * num4;
                                        int dueDay = ___market.GetPort().GetDueDay(port, gameObject.GetComponent<Good>());
                                        Mission mission = new Mission(___market.GetPort(), port, gameObject, num4, num10, 1f, 0, dueDay);
                                        list.Add(mission);
                                    }
                                }
                            }
                        }
                    }
                }
                list.Sort(SortMissions);
                list = __instance.InvokePrivateMethod<List<Mission>>("PruneNoSupplyMissions", list);
                int count = list.Count;
                ___market.GetPort().SetMissonCount(count);
                MissionStoring missionStoringcomponent = __instance.gameObject.GetComponent<MissionStoring>();
                missionStoringcomponent.page = num;
                missionStoringcomponent.missions = list;
                for (int l = 0; l < array.Length; l++)
                {
                    if (l + num < list.Count)
                    {
                        array[l] = list[l + num];
                    }
                }
                for (int m = 0; m < array.Length; m++)
                {
                    if (array[m] != null)
                    {
                        Currency region = (Currency)array[m].destinationPort.region;
                        array[m].totalPrice = CurrencyMarket.instance.GetSellPriceInCurrency(region, (float)array[m].totalPrice, false);
                        array[m].pricePerKm = (float)array[m].totalPrice / array[m].distance;
                    }
                }
                MissionSortButtonPatches.SortMissions(array);
                __result = array;
                return false;
            }

            private static int SortMissions(Mission s2, Mission s1)
            {
                switch (GPMissionSortButton.missionSorting)
                {
                    case MissionSorting.PricePerMile:
                        {
                            return s1.pricePerKm.CompareTo(s2.pricePerKm);
                        }
                    case MissionSorting.TotalPrice:
                        {
                            return s1.totalPrice.CompareTo(s2.totalPrice);
                        }
                    case MissionSorting.GoodCount:
                        {
                            return s1.goodCount.CompareTo(s2.goodCount);
                        }
                    case MissionSorting.Distance:
                        {
                            return s1.distance.CompareTo(s2.distance);
                        }
                    default:
                        {
                            return s1.pricePerKm.CompareTo(s2.pricePerKm);
                        }
                }
            }
        }
    }
}
