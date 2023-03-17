using HarmonyLib;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AutoSaveDirector))]
internal static class Patch_AutoSaveDirector
{
    [HarmonyPatch(nameof(AutoSaveDirector.SaveGame))]
    [HarmonyPrefix]
    public static bool SaveGame()
    {
        if (SRLEMod.Instance.IsBuildMode)
        {
            //TODO Saving World
            //World.Save();
            return false;
        }
        return true;
    }

    [HarmonyPatch(nameof(AutoSaveDirector.OnGameLoaded))]
    [HarmonyPrefix]
    public static void OnGameLoaded()
    {
        if (SRLEMod.Instance.IsBuildMode)
        {
            GameObject worldObj = new GameObject("World");
            worldObj.hideFlags |= HideFlags.HideAndDontSave;

        }
    }
}