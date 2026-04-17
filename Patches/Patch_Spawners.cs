using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches
{
    [HarmonyPatch]
    public class Patch_Spawners
    {
        [HarmonyPatch(typeof(DirectedAnimalSpawner), nameof(DirectedAnimalSpawner.Register)), HarmonyPrefix]
        public static bool RegisterAnimals(DirectedAnimalSpawner __instance)
        {
            if (LevelManager.IsActive)
                return !ObjectManager.GetBuildObject(__instance.gameObject, out _);
            return true;
        }

        [HarmonyPatch(typeof(DirectedActorSpawner), nameof(DirectedActorSpawner.CanSpawn)), HarmonyPrefix]
        public static bool CanSpawn(ref bool __result)
        {
            if (LevelManager.IsActive)
                if (SRLECamera.Instance && SRLECamera.Instance.gameObject.activeSelf)
                {
                    __result = false;
                    return false;
                }

            return true;
        }
    }
}
