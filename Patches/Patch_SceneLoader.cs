using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
public static class Patch_SceneLoader
{
    public static void Prefix(SceneLoader __instance, ref SceneGroup sceneGroup, SceneLoadingParameters parameters = null)
    {
        if (!SRLEMod.IsSceneLoaderPatch) return;
        SRLEMod.IsSceneLoaderPatch = false;
        sceneGroup = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
        var defaultGameplayScene = __instance.defaultGameplaySceneGroup;
        sceneGroup.coreSceneReference = defaultGameplayScene.coreSceneReference; 
    }
}