using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Regions;
using MelonLoader;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.ForceUpdate))]
public static class Patch_RegionLoader
{
    public static bool Prefix(RegionLoader __instance)
    {
        if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD && SRLECamera.Instance != null)
        {
            
            var srleCamera = SRLECamera.Instance;
            if (srleCamera.isActiveAndEnabled)
            {
                var position = srleCamera.transform.position;
                __instance.UpdateProxied(position);
                __instance.UpdateHibernated(position);
                __instance.lastRegionCheckPos = position;
                return false;
            }
            return true;
        }
        return true;
    }
}