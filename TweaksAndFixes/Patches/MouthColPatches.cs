using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class MouthColPatches
    {
        [HarmonyPatch(typeof(MouthCol), "OnTriggerEnter")]
        private static class MouthColOnTriggerEnter
        {
            [HarmonyPrefix]
            public static bool Prefix(Collider other, ref ShipItemFood ___currentFood)
            {
                var food = other.GetComponent<ShipItemFood>();
                if (food && food.sold && food.held)
                    ___currentFood = food;
                return false;
            }
        }

        [HarmonyPatch(typeof(BottleDrinking), "OnTriggerEnter")]
        private static class BottleDrinkingOnTriggerEnter
        {
            [HarmonyPrefix]
            public static bool Prefix(Collider other, ShipItemBottle ___bottle)
            {
                if (other.GetComponent<MouthCol>() && ___bottle.sold && ___bottle.held)
                    ___bottle.TryDrinkBottle();
                return false;
            }
        }
    }
}
