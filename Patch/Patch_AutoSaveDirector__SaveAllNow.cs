using HarmonyLib;
using SRML.Console;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(AutoSaveDirector), nameof(AutoSaveDirector.SaveAllNow))]
    public class Patch_AutoSaveDirector__SaveAllNow
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