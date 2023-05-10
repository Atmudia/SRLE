using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using Newtonsoft.Json;
using SRLE.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SRLE.Patches;



    [HarmonyPatch(typeof(UnityEngine.Debug))]
    internal static class Patch_Debug
    {
        public static int index = 0;
        public static bool IsConverting = true;
        public static bool isCreating = true;

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
                
                if (message.ToString().Contains("AllZones") && isCreating)
                {
                    
                    isCreating = false;
                    
                    if (!IsConverting)
                        SRLEConverterUtils.ConvertToBuildObjects();
                    
                    foreach (var scene in SRLEConverterUtils.GetAllScenes())
                    {
                        if (!SRLEManager.SceneIdClassMapping.TryGetValue(scene.name, out var list)) continue;
                        foreach (var idClass in list)
                        {
                            var gameObjectPath = GameObject.Find(idClass.Path);
                            gameObjectPath.SetActive(false);
                            var instantiate = Object.Instantiate(gameObjectPath, SRLEManager.AllZonesObj.transform);
                            instantiate.name = $"{idClass.Id} {gameObjectPath.name}";
                            instantiate.transform.position = Vector3.zero;
                               
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
        