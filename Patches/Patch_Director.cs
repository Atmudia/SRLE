using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.Popup;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(PopupDirector), nameof(PopupDirector.ShowPopups))]
public static class Patch_PopupDirector
{
    public static bool Prefix() => LevelManager.CurrentMode != LevelManager.Mode.BUILD;
}
[HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.PopAndShowFromQueue))]
public static class Patch_TutorialDirector
{
    public static bool Prefix() =>
        LevelManager.CurrentMode switch
        {
            LevelManager.Mode.NONE => true,
            LevelManager.Mode.BUILD => false,
            LevelManager.Mode.TEST => false,
            _ => throw new ArgumentOutOfRangeException()
        };
}
[HarmonyPatch(typeof(InputDirector), nameof(InputDirector.Update))]
public static class Patch_InputDirector
{
    public static bool Prefix(InputDirector __instance)
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.NONE) return true;
        
        if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
        {
            if (__instance._screenshot.triggered)
            {
                SRSingleton<GameContext>.Instance.TakeScreenshot();
            }
            __instance._mainGame.Disable();
            __instance._paused.Enable();
            return false;
        }
        return true;
    }
}