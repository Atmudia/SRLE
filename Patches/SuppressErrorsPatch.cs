using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using MelonLoader;

namespace SRLE.Patches;

[HarmonyPatch]
public static class SuppressErrorsPatch
{
    [HarmonyPatch(typeof(SECTR_PointSource), nameof(SECTR_PointSource.Play)), HarmonyPrefix]
    public static bool Patch_SECTR_PointSource_Play()
    {
        return !LevelManager.IsLoading;
    }
    [HarmonyPatch(typeof(LightControlProximity), nameof(LightControlProximity.Update)), HarmonyPrefix]
    public static bool Patch_LightControlProximity_Update(LightControlProximity __instance)
    {
        return !LevelManager.IsLoading;
    }
}