using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Components;
using SRLE.SaveSystem;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.ForceUpdate))]
    public static class Patch_RegionLoader_ForceUpdate
    {
        public static bool Prefix(RegionLoader __instance)
        {
            if (SRLEManager.isSRLELevel)
            {
                if (SRLEManager.currentData is {worldType: WorldType.SEA}) return true;
                var srleCamera = SRSingleton<SRLECamera>.Instance;
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
}