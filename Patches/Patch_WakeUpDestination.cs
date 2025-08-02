using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(WakeUpDestination))]
    public class Patch_WakeUpDestination
    {
        [HarmonyPatch(nameof(WakeUpDestination.GetRegionSetId)), HarmonyPrefix]
        public static bool GetRegionSetId(WakeUpDestination __instance, ref RegionRegistry.RegionSetId __result)
        {
            if (ObjectManager.GetBuildObject(__instance.gameObject, out var buildObject))
            {
                __result = buildObject.Region;
                return false;
            }
            return true;
        }
        
    }
}