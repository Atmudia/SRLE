using System;
using HarmonyLib;
using JetBrains.Annotations;
using SRLE.Components;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(PopupDirector), nameof(PopupDirector.MaybePopupNext))]
    internal static class Patch_PopupDirector
    {
        public static bool Prefix() => LevelManager.CurrentMode != LevelManager.Mode.BUILD;
    }
    [HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.MaybePopupNext))]
    internal static class Patch_TutorialDirector
    {
        public static bool Prefix() =>
            LevelManager.CurrentMode switch
            {
                LevelManager.Mode.NONE => true,
                LevelManager.Mode.BUILD => false,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
    [HarmonyPatch(typeof(IntroUI))]
    internal class Patch_IntroUI
    {
        [HarmonyPatch(nameof(IntroUI.Awake)), HarmonyPrefix]
        public static bool SpawnIntroSequence(IntroUI __instance)
        {
            if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
                UnityEngine.Object.Destroy(__instance.gameObject);
            return LevelManager.CurrentMode != LevelManager.Mode.BUILD;
        }
    }
}