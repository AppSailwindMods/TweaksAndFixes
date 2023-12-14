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
    internal static class ShipItemMiscPatches
    {
        [HarmonyPatch(typeof(ShipItem), "Awake")]
        private static class AwakePatch
        {
            // Token: 0x06000032 RID: 50 RVA: 0x00002C90 File Offset: 0x00000E90
            [HarmonyPrefix]
            public static void Prefix(ShipItem __instance)
            {
                __instance.gameObject.AddComponent<ShipItemInventory>();
                __instance.gameObject.AddComponent<ShipItemData>();
                if (__instance is ShipItemLampHook)
                {
                    __instance.gameObject.AddComponent<ShipItemLampHookItemHooked>();
                    return;
                }
                /*if (__instance.name.ToLower().Contains("map"))
                {
                    __instance.gameObject.AddComponent<ShipItemMoveOnAltActivate>();
                    __instance.gameObject.AddComponent<ShipItemRotateOnAltActivate>().targetAngle = 90f;
                }*/
            }
        }

        [HarmonyPatch(typeof(ShipItemLight), "SetLight")]
        private static class SetLightPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ShipItemLight __instance, bool ___on)
            {
                __instance.GetComponent<ShipItemData>().SetData("lightOn", ___on);
            }
        }

        [HarmonyPatch(typeof(ShipItemLight), "OnLoad")]
        private static class OnLoadPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipItemLight __instance, ref bool ___fixLookFlickering, Material ___paperOffMat, ref Material ___paperOnMat, Renderer ___paperRenderer)
            {
                if (___paperOffMat)
                {
                    if (___paperRenderer == null)
                    {
                        Debug.LogError("ShipItemLight: paperRenderer not found.");
                    }

                    ___paperOnMat = ___paperRenderer.sharedMaterial;
                }

                __instance.InvokePrivateMethod("SetLight", __instance.GetComponent<ShipItemData>().GetData("lightOn", false));
                ___fixLookFlickering = true;
                return false;
            }
        }
    }
}
