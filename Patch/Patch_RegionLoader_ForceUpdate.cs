using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Components;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(RegionLoader), nameof(RegionLoader.ForceUpdate))]
    public class Patch_RegionLoader_ForceUpdate
    {
        public static bool Prefix(RegionLoader __instance)
        {
            if (SRLEManager.isSRLELevel)
            {
                var srleCamera = SRSingleton<SRLECamera>.Instance;
                if (srleCamera.isActiveAndEnabled)
                {
                    __instance.UpdateProxied(srleCamera.transform.position);
                    __instance.UpdateHibernated(srleCamera.transform.position);
                    __instance.lastRegionCheckPos = srleCamera.transform.position;
                    return false;
                }
                return true;
            }

            return true;
        }
    }
}