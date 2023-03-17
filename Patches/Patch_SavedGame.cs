using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SavedGame))]
public class Patch_SavedGame
{
    [HarmonyPatch(nameof(SavedGame.CreateNew))]
    [HarmonyPrefix]
    public static bool CreateNew() => !SRLEMod.Instance.IsBuildMode;
}