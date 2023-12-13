using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweaksAndFixes.Patches
{
    internal static class DisableSwitchCamPatches
    {
        [HarmonyPatch(typeof(BoatCamera), "Update")]
        private static class Update
        {
            [HarmonyPrefix]
            public static bool Prefix(BoatCamera __instance)
            {
                if (Utilities.GamePaused) return false;
                if (GameInput.GetKeyDown(InputName.CameraMode) && !GameState.currentBoat) return false;
                return true;
            }
        }
    }
}
