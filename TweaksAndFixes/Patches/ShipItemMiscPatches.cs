using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;

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
    }
}
