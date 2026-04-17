using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(TeleportDestination))]
    public static class Patch_TeleportDestination
    {
        [HarmonyPatch(nameof(TeleportDestination.Awake)), HarmonyPrefix]
        public static bool Awake(TeleportDestination __instance)
        {
            if (LevelManager.IsActive)
                if (ObjectManager.GetBuildObject(__instance.gameObject, out var buildObject))
                {
                    SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(__instance);
                    __instance.regionSetId = buildObject.Region;
                    return false;
                }

            return true;

        }
    }
}