using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace SRLE.Patches;



    [HarmonyPatch(typeof(UnityEngine.Debug))]
    internal static class Patch_Debug
    {

        internal static IEnumerable<Scene> GetAllScenes()
        {
            for (int index = 0; index < SceneManager.sceneCount; ++index)
                yield return SceneManager.GetSceneAt(index);
        }

        [HarmonyPatch(nameof(Debug.LogWarning), typeof(Object)), HarmonyPrefix]
        public static bool DebugLogWarning(Il2CppSystem.Object message)
        {
            if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
            {
                var s = message.ToString();
                if (s.StartsWith("Instance: ") || s.Equals("Global max audio instances exceeded.")) 
                {
                    return false;
                }
            }
            return true;
        }
        
         
        [HarmonyPatch( nameof(Debug.Log), typeof(Object)), HarmonyPrefix]
        public static void DebugLogObject(Il2CppSystem.Object message)
        {
            
            
            if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
            {
                if (SRLEObjectManager.CachedGameObjects != null) return;
                if (message.ToString().Contains("AllZones"))
                {
                    if (SRLEConverter.IsConverting)
                    {
                        SRLEConverter.ConvertToBuildObjects();
                        return;
                    }
                    
                    
                    SRLEObjectManager.CachedGameObjects = new GameObject(nameof(SRLEObjectManager.CachedGameObjects))
                    { 
                        hideFlags = HideFlags.HideAndDontSave
                    };
                    Object.DontDestroyOnLoad(SRLEObjectManager.CachedGameObjects);
                  
                    
                    foreach (var scene in GetAllScenes())
                    {
                        var idClasses = SRLEObjectManager.BuildObjectsData.Where(x => x.Value.Scene.Equals(scene.name));
                        foreach (var idClass in idClasses)
                        {
                            
                            var gameObjectPath = GameObject.Find(idClass.Value.Path);
                            gameObjectPath.SetActive(false);
                            var instantiate = Object.Instantiate(gameObjectPath, SRLEObjectManager.CachedGameObjects.transform);
                            instantiate.name = $"{idClass.Value.Id} {gameObjectPath.name}";
                            instantiate.transform.position = Vector3.zero;
                            var meshFilters = instantiate.GetComponents<MeshFilter>().ToList();
                            meshFilters.AddRange(instantiate.GetComponentsInChildren<MeshFilter>());
                            foreach (var meshFilter in meshFilters.Where(meshFilter => meshFilter.GetComponent<Collider>() == null))
                                meshFilter.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
                            var skinnedMeshRenderers = instantiate.GetComponents<SkinnedMeshRenderer>().ToList();
                            skinnedMeshRenderers.AddRange(instantiate.GetComponentsInChildren<SkinnedMeshRenderer>());
                            foreach (var skinnedMeshRenderer in skinnedMeshRenderers.Where(skinnedMeshRenderer => skinnedMeshRenderer.GetComponent<Collider>() == null))
                                skinnedMeshRenderer.gameObject.AddComponent<MeshCollider>().sharedMesh = skinnedMeshRenderer.sharedMesh;
                            gameObjectPath.SetActive(true);
                            idClass.Value.GameObject = instantiate;
                        }
                    }
                    SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(SRSingleton<SystemContext>.Instance.SceneLoader.DefaultGameplaySceneGroup, new SceneLoadingParameters()
                    {
                        TeleportPlayer = true,
                        OnSceneGroupLoadedPhase2 = new Action<Il2CppSystem.Action<SceneLoadErrorData>>(data =>
                        {
                        }),
                    });
                }
                
            }
        }
    }
        