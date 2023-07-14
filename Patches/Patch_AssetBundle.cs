using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.Unload))]
public static class Patch_AssetBundle
{
    public static void Prefix(AssetBundle __instance, ref bool unloadAllLoadedObjects) => unloadAllLoadedObjects = false;
}