using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(GordoEat))]
    public class Patch_GordoEat
    {
        [HarmonyPatch(nameof(GordoEat.CanEat)), HarmonyPrefix]
        public static bool CanEat(ref bool __result)
        {
            if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}