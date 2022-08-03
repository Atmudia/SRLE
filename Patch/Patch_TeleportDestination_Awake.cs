using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Components;
using SRML.Console;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(TeleportDestination), nameof(TeleportDestination.Awake))]
    public class Patch_TeleportDestination_Awake
    {
        public static bool Prefix(TeleportDestination __instance)
        {
            if (__instance.GetComponentInParent<ObjectAddedBySRLE>())
            {
                SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(__instance);
                __instance.regionSetId = RegionRegistry.RegionSetId.HOME;

                return false;
            }
            

            return true;
        }
    }
}