﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;

namespace TweaksAndFixes.Patches
{
    internal static class PreventMugSpillagePatches
    {
        [HarmonyPatch(typeof(Mug), "Spill")]
        private static class SpillPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipItemBottle ___bottle)
            {
                return !___bottle.GetComponent<ShipItemInventory>().inInventory;
            }
        }
    }
}
