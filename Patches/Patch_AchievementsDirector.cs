using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector))]
    public class Patch_AchievementsDirector
    {
        [HarmonyPatch(nameof(AchievementsDirector.Update)), HarmonyPrefix]
        public static bool Update()
        {
            return !LevelManager.IsActive;
        }
        [HarmonyPatch(nameof(AchievementsDirector.LateUpdate)),  HarmonyPrefix]
        public static bool LateUpdate()
        {
            return !LevelManager.IsActive;
        }
    }
}