using HarmonyLib;

namespace SRLE.Patches;

[HarmonyPatch(typeof(PopupDirector), nameof(PopupDirector.MaybePopupNext))]
public static class Patch_PopupDirector
{
    public static bool Prefix() => !SRLEMod.Instance.IsBuildMode;
}
[HarmonyPatch(typeof(TutorialDirector), nameof(TutorialDirector.PopAndShowFromQueue))]
public static class Patch_TutorialDirector
{
    public static bool Prefix() => !SRLEMod.Instance.IsBuildMode;
}