using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class ShipItemLampHookPatches
    {
        [HarmonyPatch(typeof(ShipItemLampHook), "OnEnterInventory")]
        private static class OnEnterInventoryPatch
        {
            // Token: 0x06000045 RID: 69 RVA: 0x00003058 File Offset: 0x00001258
            [HarmonyPrefix]
            public static void Prefix(ShipItemLampHook __instance, ref bool ___occupied)
            {
                ShipItemLampHookItemHooked component = __instance.GetComponent<ShipItemLampHookItemHooked>();
                if ((component && component.hookedItem) & ___occupied)
                {
                    component.hookedItem.GetComponent<HangableItem>().DisconnectJoint();
                    component.hookedItem = null;
                }
            }
        }

        [HarmonyPatch(typeof(ShipItemLampHook), "OnItemClick")]
        private static class OnItemClickPatch
        {
            // Token: 0x06000046 RID: 70 RVA: 0x000030A8 File Offset: 0x000012A8
            [HarmonyPrefix]
            public static bool Prefix(ShipItemLampHook __instance, PickupableItem heldItem, ref bool ___occupied, ref bool __result)
            {
                if (!__instance.sold | ___occupied)
                {
                    __result = false;
                    return false;
                }
                HangableItem component = heldItem.GetComponent<HangableItem>();
                if (component && heldItem.GetComponent<ShipItem>().sold)
                {
                    component.ConnectJoint(__instance.GetComponent<Collider>());
                    __instance.GetComponent<ShipItemLampHookItemHooked>().hookedItem = heldItem;
                }
                __result = true;
                return false;
            }
        }
    }
}
