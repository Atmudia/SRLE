using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Update))]
    public class Patch_SpawnResource_Update
    {
        public static bool Prefix() => !SRLEManager.isSRLELevel;
    }
}