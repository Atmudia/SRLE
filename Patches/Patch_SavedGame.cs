using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SavedGame))]
public static class Patch_SavedGame
{
    [HarmonyPatch(nameof(SavedGame.CreateNew))]
    [HarmonyPrefix]
    public static bool CreateNew() => LevelManager.CurrentMode != LevelManager.Mode.TEST;
}