using HarmonyLib;
using MelonLoader;

namespace SRLE.Patches;

[HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
internal static class Patch_Il2CppDetourMethodPatcher
{
    public static bool Prefix(System.Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;                               
    }
}