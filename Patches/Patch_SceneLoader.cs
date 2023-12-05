using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using SRLE.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
public static class Patch_SceneLoader
{
    public static void Prefix(SceneLoader __instance, ref SceneGroup sceneGroup, SceneLoadingParameters parameters)
    {
        if (!SRLEMod.IsSceneLoaderPatch) return;
        SRLEMod.IsSceneLoaderPatch = false;
        
        sceneGroup = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
        var defaultGameplayScene = __instance._defaultGameplaySceneGroup;
        sceneGroup._coreSceneReference = defaultGameplayScene.CoreSceneReference;
        //sceneGroup = EntryPoint.VoidGroup;

        /*SceneManager.LoadSceneAsync(SRLEObjectManager.EmptyLevelPath, new LoadSceneParameters()
        {

            loadSceneMode = LoadSceneMode.Additive
        });
        */


    }
}