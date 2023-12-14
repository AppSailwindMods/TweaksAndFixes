using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SailwindModdingHelper;
using System.Collections.Generic;
using System.Reflection;
using TweaksAndFixes.Patches;
using UnityEngine;

namespace TweaksAndFixes
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(SailwindModdingHelperMain.GUID, "2.0.2")]
    public class TweaksAndFixesMain : BaseUnityPlugin
    {
        public const string GUID = "com.app24.tweaksandfixes";
        public const string NAME = "Tweaks And Fixes";
        public const string VERSION = "2.0.1";

        internal static ManualLogSource logSource;

        internal static TweaksAndFixesMain instance;

        private ConfigEntry<KeyboardShortcut> quickSaveKey;
        internal ConfigEntry<float> animationTime;
        internal ConfigEntry<bool> skipDisclaimer;

        internal SaveContainer saveContainer;

        private void Awake()
        {
            instance = this;
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            quickSaveKey = Config.Bind("Hotkeys", "Quick Save", new KeyboardShortcut(KeyCode.F8));
            animationTime = Config.Bind("Values", "Animation Time", 7f, "The time it takes the camera to move to position when starting a new game");
            skipDisclaimer = Config.Bind("Values", "Skip Disclaimer", false);

            GameEvents.OnPlayerInput += (_, __) =>
            {
                if (quickSaveKey.Value.IsDown() && SaveLoadManager.readyToSave)
                {
                    SaveLoadManager.instance.SaveGame(true);
                    NotificationUi.instance.ShowNotification("Game Saved!");
                }
            };

            GameEvents.OnGameStart += (_, __) =>
            {
                QuitNoSaveData.SetUpButton();
            };

            GameEvents.OnNewGame += (_, __) =>
            {
                saveContainer = new SaveContainer();
            };

            GameEvents.OnGameSave += (_, __) =>
            {
                if (saveContainer == null) saveContainer = new SaveContainer();
                ModSave.Save(Info, saveContainer);
                SaveLoadManagerPatches.AddPrefabPatch.indexCounter = 0;
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs = new Dictionary<SaveablePrefab, int>();
            };

            GameEvents.OnSaveLoad += (_, __) =>
            {
                if (!ModSave.Load<SaveContainer>(Info, out saveContainer))
                {
                    saveContainer = new SaveContainer();
                }
                if (saveContainer.itemDatas == null)
                {
                    saveContainer.itemDatas = new List<ItemDataSaveable>();
                }
                SaveablePrefabPatches.LoadPatch.AssignHookItems();
            };
        }
    }
}
