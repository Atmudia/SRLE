using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.Unload))]
internal static class Patch_AssetBundle
{
    public static void Prefix(AssetBundle __instance, ref bool unloadAllLoadedObjects)
    {
        // if (LevelManager.CurrentMode != LevelManager.Mode.NONE)
        {
            unloadAllLoadedObjects = false;
        }
        // else
        // {
        //     MelonLogger.Msg($"Unloading assetbundle: {__instance.name}");
        // }
    }
}