using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;

namespace TweaksAndFixes.Patches
{
    internal static class SaveablePrefabPatches
    {
        [HarmonyPatch(typeof(SaveablePrefab), "Load")]
        public static class LoadPatch
        {
            public static void Postfix(SaveablePrefab __instance, ShipItem ___item, SavePrefabData data)
            {
                indexCounter++;
                saveablePrefabs.Add(indexCounter, __instance);
            }

            public static void AssignHookItems()
            {
                TweaksAndFixesMain.instance.saveContainer.hookItems.ForEach(h =>
                {
                    if (saveablePrefabs.TryGetValue(h.prefabIndex, out var prefab))
                    {
                        if (saveablePrefabs.TryGetValue(h.hookItemIndex, out var hookItem))
                        {
                            prefab.GetComponent<ShipItemLampHookItemHooked>().hookedItem = hookItem.GetComponent<PickupableItem>();
                        }
                    }
                });
                TweaksAndFixesMain.instance.saveContainer.inInventoryItems.ForEach(i =>
                {
                    if (saveablePrefabs.TryGetValue(i.prefabIndex, out var prefab))
                    {
                        prefab.GetComponent<ShipItemInventory>().inInventory = i.inInventory;
                    }
                });
                TweaksAndFixesMain.instance.saveContainer.itemDatas.ForEach(i =>
                {
                    if (saveablePrefabs.TryGetValue(i.prefabIndex, out var prefab))
                    {
                        prefab.GetComponent<ShipItemData>().SetData(i.dataName, i.data);
                    }
                });
            }

            public static Dictionary<int, SaveablePrefab> saveablePrefabs = new Dictionary<int, SaveablePrefab>();
            public static int indexCounter;
        }

        [HarmonyPatch(typeof(SaveablePrefab), "PrepareSaveData")]
        public static class PrepareSaveDataPatch
        {
            public static void Prefix(ShipItem ___item)
            {
                indexCounter++;
                ShipItemLampHookItemHooked component = ___item.GetComponent<ShipItemLampHookItemHooked>();
                if (component && component.hookedItem)
                {
                    SaveablePrefab saveablePrefab = component.hookedItem.GetComponent<SaveablePrefab>();
                    if (saveablePrefab && saveablePrefabs.TryGetValue(saveablePrefab, out int index))
                    {
                        TweaksAndFixesMain.instance.saveContainer.hookItems.Add(new HookItemSaveable(indexCounter, index));
                    }
                }
                ShipItemInventory shipItemInventory = ___item.GetComponent<ShipItemInventory>();
                if (shipItemInventory)
                {
                    TweaksAndFixesMain.instance.saveContainer.inInventoryItems.Add(new InInventorySaveable(indexCounter, shipItemInventory.inInventory));
                }
                ShipItemData shipItemData = ___item.GetComponent<ShipItemData>();
                if (shipItemData)
                {
                    foreach (var keyValuePair in shipItemData.data)
                    {
                        TweaksAndFixesMain.instance.saveContainer.itemDatas.Add(new ItemDataSaveable(indexCounter, keyValuePair.Key, keyValuePair.Value));
                    }
                }
            }

            public static Dictionary<SaveablePrefab, int> saveablePrefabs = new Dictionary<SaveablePrefab, int>();
            public static int indexCounter;
        }
    }
}
