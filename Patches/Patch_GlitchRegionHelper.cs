using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(GlitchRegionHelper))]
    public static class Patch_GlitchRegionHelper
    {
        [HarmonyPatch(nameof(GlitchRegionHelper.RegionSetChanged))]
        public static bool Prefix()
        {
            return !LevelManager.IsActive;
        }
    }
}