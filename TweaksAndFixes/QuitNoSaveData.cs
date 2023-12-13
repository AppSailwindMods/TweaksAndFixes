using HarmonyLib;
using SailwindModdingHelper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;
using UnityEngine;

namespace TweaksAndFixes
{
    internal static class QuitNoSaveData
    {
        private static GameObject quitWithoutSavingButton;

        private static GameObject quitButton;

        private static Vector3 originalQuitPos;

        public static void SetUpButton()
        {
            StartMenu startMenu = Object.FindObjectOfType<StartMenu>();
            if (startMenu)
            {
                GameObject gameObject = startMenu.GetPrivateField<GameObject>("confirmQuitUI");
                foreach (Transform transform in gameObject.transform)
                {
                    if (transform.name == "button quit")
                    {
                        quitButton = transform.gameObject;
                        originalQuitPos = transform.localPosition;
                        quitWithoutSavingButton = Object.Instantiate<GameObject>(transform.gameObject);
                        quitWithoutSavingButton.name = "button quit no save";
                        quitWithoutSavingButton.transform.parent = gameObject.transform;
                        quitWithoutSavingButton.transform.localPosition = new Vector3(-0.486f, -0.311f, 0.036f);
                        quitWithoutSavingButton.transform.localRotation = Quaternion.Euler(180f, 1.366038E-05f, 180f);
                        quitWithoutSavingButton.transform.localScale = Vector3.one;
                        GameObject gameObject2 = quitWithoutSavingButton.GetComponentInChildren<StartMenuButton>().gameObject;
                        Object.Destroy(gameObject2.GetComponent<StartMenuButton>());
                        gameObject2.AddComponent<GPQuitNoSaveButton>().startMenu = startMenu;
                        quitWithoutSavingButton.GetComponentInChildren<TextMesh>().text = "Quit No Save";
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StartMenu), "EnableQuitConfirmMenu")]
        private static class EnableQuitConfirmMenu
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                quitWithoutSavingButton.SetActive(GameState.playing);
                quitButton.SetActive(false);
                quitButton.transform.localPosition = (GameState.playing ? new Vector3(0.486f, -0.311f, 0.036f) : originalQuitPos);
                quitButton.SetActive(true);
            }
        }
    }
}
