using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
public static class Patch_SceneLoader
{

    public static List<Component> Components = new List<Component>(); 

    internal static IEnumerable<Scene> GetAllScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; ++index)
            yield return SceneManager.GetSceneAt(index);
    }
    public static void Prefix(SceneLoader __instance,ref SceneGroup sceneGroup, SceneLoadingParameters parameters)
    {
        // return;
        if (!LevelManager.IsLoading)
            return;
        if (ObjectManager.CachedGameObjects != null && ObjectManager.CachedGameObjects.transform.GetChildCount() != 0)
            return;


        var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<SceneGroup>();
        sceneGroup = findObjectsOfTypeAll.FirstOrDefault(x => x.name.Contains("AllZones"));
        var defaultGameplayScene = __instance._defaultGameplaySceneGroup;
        var labyrinth = findObjectsOfTypeAll.FirstOrDefault(x => x.name.Contains("Labyrinth"))._sceneReferences.ToList();
        // sceneGroup = ScriptableObject.CreateInstance<SceneGroup>();
        var assetReferences = sceneGroup._sceneReferences.ToList();
        assetReferences.AddRange(labyrinth);
        sceneGroup._sceneReferences = assetReferences.ToArray();
        sceneGroup._coreSceneReference = defaultGameplayScene.CoreSceneReference;

        // var ConservatoryFields = findObjectsOfTypeAll.FirstOrDefault(x => x.name.Contains("ConservatoryFields"))._sceneReferences.ToList();
        // ConservatoryFields.AddRange(findObjectsOfTypeAll.FirstOrDefault(x => x.name.Contains("LuminousStrand"))._sceneReferences);
        // sceneGroup._sceneReferences = ConservatoryFields.ToArray();
        // sceneGroup.
        parameters.TeleportPlayer = true;
        parameters.OnSceneGroupLoadedPhase2 += new System.Action<Il2CppSystem.Action<SceneLoadErrorData>>(action =>
        {
            if (SRLEConverter.IsConverting)
            {
                SRLEConverter.ConvertToBuildObjects();
                return;
            }
            ObjectManager.CachedGameObjects = new GameObject(nameof(ObjectManager.CachedGameObjects))
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            
            Object.DontDestroyOnLoad(ObjectManager.CachedGameObjects);
            foreach (var scene in GetAllScenes())
            {
                var idClasses = ObjectManager.BuildObjectsData.Where(x => x.Value.Scene.Equals(scene.name));
                foreach (var idClass in idClasses)
                {
                    var gameObjectPath = GameObject.Find(idClass.Value.Path);
                    var beforeState = gameObjectPath.activeInHierarchy;
                    gameObjectPath.SetActive(false);
                    
                    var instantiate = Object.Instantiate(gameObjectPath, new Vector3(0,0,0), gameObjectPath.transform.rotation, ObjectManager.CachedGameObjects.transform);
                  
                    
                    instantiate.name = $"{idClass.Value.Id} {gameObjectPath.name}";
                    // instantiate.transform.position = Vector3.zero;
                    // instantiate.transform.localPosition = Vector3.zero;

                    gameObjectPath.SetActive(beforeState);
                    idClass.Value.GameObject = instantiate;
                    
                    
                    // if (instantiate.name.Contains("nodeSlime"))
                    // {
                    //     instantiate.AddComponent<BoxCollider>();
                    // }
                    //
                    // MeshFilter[] filters = instantiate.GetComponentsInChildren<MeshFilter>().Where(x => x.sharedMesh != null).ToArray();
                    //
                    // foreach (var meshFilter in filters)
                    // {
                    //     var addComponent = meshFilter.gameObject.AddComponent<MeshCollider>();
                    //     addComponent.convex = true;
                    //     addComponent.sharedMesh = meshFilter.sharedMesh;
                    // }

                    // instantiate.AddComponent<PrefabColliderGenerator>();
                }
            }

        });
    }
    
}

