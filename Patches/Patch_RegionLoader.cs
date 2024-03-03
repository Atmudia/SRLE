using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Regions;
using MelonLoader;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.Update))]
public static class Patch_RegionLoader
{
    public static bool Prefix(RegionLoader __instance)
    {
        var srleCamera = SRLECamera.Instance;
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD && srleCamera != null)
        {
            if (!srleCamera.isActiveAndEnabled)
                return true;
            var position = srleCamera.transform.position;
            __instance.UpdateProxied(position);
            __instance.UpdateHibernated(position);
            return false;
        }
        return false;
    }
}