using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(DirectedActorSpawner), nameof(DirectedActorSpawner.CanSpawnSomething))]
public static class Patch_DirectedActorSpawner
{
    public static bool Prefix(DirectedActorSpawner __instance, ref bool __result)
    {
        if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
        {
            __result = false;
            return false;

        }

        // if (SRLEMod.CurrentMode != SRLEMod.Mode.BUILD) return true;
        //__instance.enabled = !__instance.enabled;
        return true;
    }
}
[HarmonyPatch(typeof(SpawnResource), nameof(SpawnResource.Update))]
public static class Patch_SpawnResource
{
    public static bool Prefix(SpawnResource __instance)
    {
        if (SRLEMod.CurrentMode != SRLEMod.Mode.BUILD) return true;
        return false;
    }
}