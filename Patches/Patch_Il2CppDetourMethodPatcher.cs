using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;

namespace SRLE.Patches
{
    [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
    public static class Patch_Il2CppDetourMethodPatcher
    {
        public static bool Prefix(System.Exception ex)
        {
            MelonLogger.Error("During invoking native->managed trampoline", ex);
            return false;                               
        }
    }
}