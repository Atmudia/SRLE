using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.VFX.Lights;
using MelonLoader;

namespace SRLE.Patches;

[HarmonyPatch]
internal static class SuppressErrorsPatch
{
    [HarmonyPatch(typeof(SECTR_PointSource), nameof(SECTR_PointSource.Play)), HarmonyPrefix]
    public static bool Patch_SECTR_PointSource_Play()
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.NONE) return true;
        return !LevelManager.IsLoading;
    }
    [HarmonyPatch(typeof(LightControlProximity), nameof(LightControlProximity.Update)), HarmonyPrefix]
    public static bool Patch_LightControlProximity_Update(LightControlProximity __instance)
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.NONE) return true;
        return !LevelManager.IsLoading;
    }
}