using System;
using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(PopupDirector), nameof(PopupDirector.MaybePopupNext))]
public static class Patch_PopupDirector
{
    public static bool Prefix() => SRLEMod.CurrentMode != SRLEMod.Mode.BUILD;
}
[HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.PopAndShowFromQueue))]
public static class Patch_TutorialDirector
{
    public static bool Prefix() =>
        SRLEMod.CurrentMode switch
        {
            SRLEMod.Mode.NONE => true,
            SRLEMod.Mode.BUILD => false,
            _ => throw new ArgumentOutOfRangeException()
        };
}