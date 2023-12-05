using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(RegionRegistry), nameof(RegionRegistry.Awake))]
public static class Patch_RegionRegistry
{
    public static void Prefix(RegionRegistry __instance)
    {
        //__instance.managedWithSceneGroup.Add(EntryPoint.VoidGroup, new List<GameObject>());
        __instance._sceneGroupQuadTrees.Add(EntryPoint.VoidGroup, new BoundsQuadtree<Region>(0, Vector3.zero, 1, 1));

    }
}