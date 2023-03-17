using HarmonyLib;

namespace SRLE.Patches;

[HarmonyPatch(typeof(KillOnTrigger), nameof(KillOnTrigger.OnTriggerEnter))]
public static class KillOnTriggerOnTriggerEnter
{
    public static bool Prefix()
    {
        //TODO Include here Anti GUI Killer
        return !Patch_Debug.disableKill;
    }
}