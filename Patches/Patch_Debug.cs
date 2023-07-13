using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace SRLE.Patches;



    [HarmonyPatch(typeof(UnityEngine.Debug))]
    internal static class Patch_Debug
    {
        public static int index = 0;
        public static bool IsConverting = false;
        public static bool isExecuted;

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
        public static bool DebugLogObject(Il2CppSystem.Object message)
        {
            if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
            {
                if (message.ToString().Contains("AllZones") && !isExecuted)
                { 
                    if (IsConverting)
                        SRLEConverterUtils.ConvertToBuildObjects();
                    isExecuted = true;
                    foreach (var scene in SRLEConverterUtils.GetAllScenes())
                    {
                        if (!SRLEManager.SceneIdClassMapping.TryGetValue(scene.name, out var list)) continue;
                        foreach (var idClass in list)
                        {
                            var gameObjectPath = GameObject.Find(idClass.Path);
                            gameObjectPath.SetActive(false);
                            var instantiate = Object.Instantiate(gameObjectPath, SRLEManager.CachedObjects.transform);
                            instantiate.name = $"{idClass.Id} {gameObjectPath.name}";
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
                            SRLEManager.GameObjectIdClassMapping.Add(idClass, instantiate);
                        }
                    }
                    SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(SRLEMod.DefaultZone, new SceneLoadingParameters()
                    {
                        teleportPlayer = true,
                        onSceneGroupLoadedPhase2 = new Action<Il2CppSystem.Action<SceneLoadErrorData>>(data =>
                        {
                            SRLESaveSystem.LoadObjectsFromLevel();
                        }),
                    });
                }
                
            }
            return true;
        }
    }
        