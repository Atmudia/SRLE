using HarmonyLib;
using MonomiPark.SlimeRancher;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    internal static class Patch_SavedGame
    {
        [HarmonyPatch(nameof(SavedGame.CreateNew))]
        [HarmonyPrefix]
        public static bool CreateNew()
        {
            return !LevelManager.IsActive;
        }
    }
}