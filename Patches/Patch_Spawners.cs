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
            if (ObjectManager.GetBuildObject(__instance.gameObject, out _))
                return false;
            return true;
        }
    }
}