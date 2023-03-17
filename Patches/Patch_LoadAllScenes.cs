using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SRLE.Patches;



    [HarmonyPatch(typeof(UnityEngine.Debug))]
    public static class Patch_Debug
    {
        public static SceneGroup[] AllSceneGroups;
        public static int index = 0;
        public static bool disableKill = false;
        public static bool IsConverting = true;

        [HarmonyPatch(nameof(Debug.LogWarning), typeof(Object))]
        [HarmonyPrefix()]
        public static bool DebugLogWarning(Il2CppSystem.Object message)
        {
            if (SRLEMod.Instance.IsBuildMode)
            {
                var s = message.ToString();
                if (s.StartsWith("Instance: "))
                {
                    return false;
                }
                if (s.Equals("Global max audio instances exceeded."))
                {
                    return false;
                }
                

            }

            return true;
        }
        [HarmonyPatch( nameof(Debug.Log), typeof(Object))]
        [HarmonyPrefix()]
        public static bool DebugLogObject(Il2CppSystem.Object message)
        {
            if (SRLEMod.Instance.IsBuildMode)
            {

                if (message.ToString().Contains("Scene Transition from"))
                {
                    if (index == 1)
                    {
                        if (disableKill)
                        {
                            foreach (var sceneGroup in Resources.FindObjectsOfTypeAll<SceneGroup>())
                            {
                                sceneGroup.showLoadingScreen = !sceneGroup.showLoadingScreen;
                            }
                        }
                        disableKill = false;   
                        return true;
                    }
                    disableKill = true;

                
                    SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones")),
                        new SceneLoadingParameters()
                        {
                            teleportPlayer = true,
                            pauseGame = false,
                            playTeleportAudio = false,
                        
                            onSceneGroupLoadedPhase2 = new Action<Il2CppSystem.Action<SceneLoadErrorData>>(action =>
                            {
                                
                                if (IsConverting)
                                    SRLEConverter.ConvertToBuildObjects();
                                
                                    
                            })
                        });
                    disableKill = false;
                
                    if (index != AllSceneGroups.Length)
                        index++;;
                }
            }

            return true;
        }
        
    }