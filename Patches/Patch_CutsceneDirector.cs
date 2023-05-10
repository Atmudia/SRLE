using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Cutscene;
using Il2CppMonomiPark.SlimeRancher.UI.IntroSequence;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(CutsceneDirector))]
public class Patch_CutsceneDirector
{
    [HarmonyPatch(nameof(CutsceneDirector.SpawnIntroSequence)), HarmonyPrefix ]
    public static bool SpawnIntroSequence() =>
        SRLEMod.CurrentMode switch
            {
                SRLEMod.Mode.NONE => true,
                SRLEMod.Mode.BUILD => false,
                SRLEMod.Mode.PLAY => true,
                _ => true
            };    
}