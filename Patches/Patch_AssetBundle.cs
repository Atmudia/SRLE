using System.Linq;
using HarmonyLib;
using MelonLoader;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.Unload))]
internal static class Patch_AssetBundle
{
    public static void Prefix(ref bool unloadAllLoadedObjects)
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD || LevelManager.CurrentMode == LevelManager.Mode.TEST)
            unloadAllLoadedObjects = false;
    }
}