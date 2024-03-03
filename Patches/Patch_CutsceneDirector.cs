using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Cutscene;

namespace SRLE.Patches;

[HarmonyPatch(typeof(CutsceneDirector))]
public class Patch_CutsceneDirector
{
    [HarmonyPatch(nameof(CutsceneDirector.SpawnIntroSequence)), HarmonyPrefix]
    public static bool SpawnIntroSequence()
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
            return false;
        return true;
    }
}