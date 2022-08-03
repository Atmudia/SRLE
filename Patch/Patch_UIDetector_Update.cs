using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(UIDetector), nameof(UIDetector.Update))]
    public static class Patch_UIDetector_Update
    {
        public static bool Prefix()
        {
            return SRSingleton<SRLECamera>.Instance?.isActiveAndEnabled != true;
        }
    }
}