using System.Linq;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.Unload))]
public static class Patch_AssetBundle
{
    public static void Prefix(AssetBundle __instance, ref bool unloadAllLoadedObjects)
    {
        if (LevelManager.CurrentMode != LevelManager.Mode.NONE)
        {
            unloadAllLoadedObjects = false;
        }
    }
}