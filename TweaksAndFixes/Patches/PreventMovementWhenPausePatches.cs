using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweaksAndFixes.Patches
{
    internal static class PreventMovementWhenPausePatches
    {
        [HarmonyPatch(typeof(PlayerClimb), "Update")]
        private static class PlayerClimbUpdatePatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !Utilities.GamePaused;
            }
        }

        [HarmonyPatch(typeof(PlayerCrouching), "Update")]
        private static class PlayerCrouchingUpdatePatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                return !Utilities.GamePaused;
            }
        }
    }
}
