using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Cutscene;
using Il2CppMonomiPark.SlimeRancher.UI.IntroSequence;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(CutsceneDirector))]
public class Patch_CutsceneDirector
{
    [HarmonyPatch(nameof(CutsceneDirector.SpawnIntroSequence))]
    [HarmonyPrefix]
    public static bool SpawnIntroSequence() => !SRLEMod.Instance.IsBuildMode;

}