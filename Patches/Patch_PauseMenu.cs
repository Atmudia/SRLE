using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(PauseMenu))]
    internal static class Patch_PauseMenu
    {
        [HarmonyPatch(nameof(PauseMenu.PauseGame)), HarmonyPrefix]
    
        public static bool TogglePause()
        {
            if (LevelManager.CurrentMode != LevelManager.Mode.BUILD) return true;
            SRLECamera.Instance.SetActive(!SRLECamera.Instance.isActiveAndEnabled);
            EntryPoint.ConsoleInstance.Log(!SRLECamera.Instance.isActiveAndEnabled);
            return false;
        }
        [HarmonyPatch(nameof(PauseMenu.Quit)), HarmonyPrefix]
        public static void Quit(PauseMenu __instance)
        {
            if (LevelManager.CurrentMode != LevelManager.Mode.BUILD) return;
            SRLECamera.Instance.SetActive(false);
            __instance.StopCoroutine(SRLECamera.Instance.DestroyDelayedObject);
        }

    }
}