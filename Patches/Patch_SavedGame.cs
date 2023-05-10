using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SavedGame))]
public class Patch_SavedGame
{
    [HarmonyPatch(nameof(SavedGame.CreateNew))]
    [HarmonyPrefix]
    public static bool CreateNew() => SRLEMod.CurrentMode != SRLEMod.Mode.BUILD;
}