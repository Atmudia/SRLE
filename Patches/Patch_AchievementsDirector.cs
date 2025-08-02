using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(AchievementsDirector))]
    public class Patch_AchievementsDirector
    {
        [HarmonyPatch(nameof(AchievementsDirector.Update))]
        public static bool Update()
        {
            if (SaveManager.CurrentLevel != null)
            {
                return false;
            }
            EntryPoint.ConsoleInstance.Log("testing");
            return true;
        }
        [HarmonyPatch(nameof(AchievementsDirector.LateUpdate))]
        public static bool LateUpdate()
        {
            if (SaveManager.CurrentLevel != null)
                return false;
            return true;
        }
    }
}