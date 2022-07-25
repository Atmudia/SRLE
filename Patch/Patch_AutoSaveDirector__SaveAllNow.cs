using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveAllNow))]
    public static class Patch_AutoSaveDirector__SaveAllNow
    {
        public static bool Prefix(ref bool __result)
        {
            var saveLevel = SRLEManager.SaveLevel();
            if (saveLevel)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}