using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Update))]
public static class Patch_KillSlimes
{
    public static bool Prefix()
    {
        if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
        {
            return false;
        }

        return true;
    }
}