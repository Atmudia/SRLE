using HarmonyLib;
using MonomiPark.SlimeRancher;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(SavedGame), nameof(SavedGame.CreateNew))]
    public class Patch_SavedGame_CreateNew
    {
        public static bool Prefix()
        {
            if (SRLEManager.isSRLELevel)
            {
                return false;
            }

            return true;
        }

    }
}