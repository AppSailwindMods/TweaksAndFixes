using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Patches;
using UnityEngine;

namespace TweaksAndFixes.Scripts
{
    internal enum MissionSorting
    {
        PricePerMile,
        TotalPrice,
        GoodCount,
        Distance,
        Last
    }

    internal class GPMissionSortButton : GoPointerButton
    {
        public TextMesh text;

        public static Port currentPort;

        public static MissionSorting missionSorting = MissionSorting.PricePerMile;
        private static string[] sortingStrings = new[]
        {
            "Price Per Mile",
            "Reward",
            "Crate Amount",
            "Distance"
        };

        public override void OnActivate()
        {
            missionSorting++;
            if (missionSorting >= MissionSorting.Last)
            {
                missionSorting = 0;
            }
            UpdateMissions();
            UpdateText();
        }

        public override void OnAltActivate()
        {
            missionSorting--;
            if (missionSorting < 0)
            {
                missionSorting = MissionSorting.Last - 1;
            }
            UpdateMissions();
            UpdateText();
        }

        private void UpdateMissions()
        {
            if (currentPort != null)
            {
                MissionListUI.instance.ChangePage(-MissionListUI.instance.GetPrivateField<int>("currentPage"));
                MissionSortButtonPatches.SortMissions(currentPort.GetMissions(MissionListUI.instance.GetPrivateField<int>("currentPage"), MissionListUI.instance.worldMissions));
                MissionDetailsUI.instance.GetPrivateField<GameObject>("UI").SetActive(false);
                MissionDetailsUI.instance.SetPrivateField("currentMission", null);
                MissionDetailsUI.instance.InvokePrivateMethod("ZoomMap", false);
                MissionListUI.instance.InvokePrivateMethod("UpdatePageCountText");
            }
        }

        public void UpdateText()
        {
            string missionSortingText = missionSorting.ToString();
            if ((int)missionSorting < sortingStrings.Length)
            {
                missionSortingText = sortingStrings[(int)missionSorting];
            }
            text.text = $"Sorting by: {missionSortingText}";
        }
    }
}
