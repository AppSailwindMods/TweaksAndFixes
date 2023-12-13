using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TweaksAndFixes.Patches
{
    internal static class SaveLoadManagerPatches
    {
        [HarmonyPatch(typeof(SaveLoadManager), "AddPrefab")]
        internal static class AddPrefabPatch
        {
            [HarmonyPrefix]
            public static void Prefix(SaveablePrefab pref)
            {
                indexCounter++;
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs.Add(pref, indexCounter);
            }

            public static int indexCounter;
        }

        [HarmonyPatch(typeof(SaveLoadManager), "RemovePrefab")]
        private static class RemovePrefabPatch
        {
            [HarmonyPrefix]
            public static void Prefix(SaveablePrefab pref)
            {
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs.Remove(pref);
            }
        }
    }
}
