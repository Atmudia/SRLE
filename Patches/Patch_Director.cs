using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Cutscene;
using Il2CppMonomiPark.SlimeRancher.Input;
using Il2CppMonomiPark.SlimeRancher.UI.Popup;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(PopupDirector), nameof(PopupDirector.ShowPopups))]
internal static class Patch_PopupDirector
{
    public static bool Prefix() => LevelManager.CurrentMode != LevelManager.Mode.BUILD;
}
[HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.PopAndShowFromQueue))]
internal static class Patch_TutorialDirector
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
[HarmonyPatch(typeof(CutsceneDirector))]
internal class Patch_CutsceneDirector
{
    [HarmonyPatch(nameof(CutsceneDirector.SpawnIntroSequence)), HarmonyPrefix]
    public static bool SpawnIntroSequence(CutsceneDirector __instance)
    {
        UnityEngine.Object.Destroy(__instance._uiRoot);
        return LevelManager.CurrentMode != LevelManager.Mode.BUILD;
    }
    // [HarmonyPatch(nameof(CutsceneDirector.RegisterIntroSequenceUIRoot)), HarmonyPrefix]
    // public static bool RegisterIntroSequenceUIRoot()
    // {
    //     return LevelManager.CurrentMode != LevelManager.Mode.BUILD;
    // }
    // [HarmonyPatch(nameof(CutsceneDirector.)), HarmonyPrefix]
    // public static bool RegisterIntroSequenceUIRoot()
    // {
    //     return LevelManager.CurrentMode != LevelManager.Mode.BUILD;
    // }
}
[HarmonyPatch(typeof(InputDirector), nameof(InputDirector.Update))]
internal static class Patch_InputDirector
{
    public static bool Prefix(InputDirector __instance)
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.NONE) return true;
        
        if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
        {
            // if (__instance._screenshot.asset.)
            // {
            //     SRSingleton<GameContext>.Instance.TakeScreenshot();
            // }
            __instance._mainGame.Map.Disable();
            __instance._paused.Map.Enable();
            // __instance._paused.Map.Enable();
            return false;
        }
        __instance._mainGame.Map.Enable();
        __instance._paused.Map.Disable();
        return true;
    }
}