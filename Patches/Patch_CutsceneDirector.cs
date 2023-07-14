using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Cutscene;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.IntroSequence;
using MelonLoader;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(CutsceneDirector))]
public class Patch_CutsceneDirector
{
    [HarmonyPatch(nameof(CutsceneDirector.SpawnIntroSequence)), HarmonyPrefix]
    public static bool SpawnIntroSequence()
    {
        if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
            return false;
        return true;
    }
}