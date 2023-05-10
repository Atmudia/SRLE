using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SRLE.Patches;




[HarmonyPatch(typeof(AssetBundle), nameof(AssetBundle.Unload))]
public static class Patch_AssetBundle
{
    public static List<AssetReference> sceneAssetReferences = new List<AssetReference>();
    public static void Prefix(AssetBundle __instance, ref bool unloadAllLoadedObjects)
    {
        unloadAllLoadedObjects = false;
    }
}