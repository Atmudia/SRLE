using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using SRLE.Models;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace SRLE.Patches;

[HarmonyPatch(typeof(Debug))]
internal static class Patch_Debug
{
    [HarmonyPatch(nameof(Debug.Log), typeof(Object)), HarmonyPrefix]
    public static void Log(Il2CppSystem.Object message)
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
        {
            if (message.ToString().Contains("AllZones") && LevelManager.IsLoading)
            {
                SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(SRSingleton<SystemContext>.Instance.SceneLoader.DefaultGameplaySceneGroup, new SceneLoadingParameters()
                {
                    TeleportPlayer = false,
                    OnSceneGroupLoadedPhase2 = new Action<Il2CppSystem.Action<SceneLoadErrorData>>(_ =>
                    {
                        SRSingleton<SceneContext>.Instance.Player.GetComponent<SRCharacterController>().Position = new Vector3(541.6444f, 18.6f, 349.3277f);
                        UIInitializer.Initialize();
                        ObjectManager.World = new GameObject("SRLEWorld");
                        ObjectManager.World.hideFlags |= HideFlags.HideAndDontSave;
                        Object.DontDestroyOnLoad(ObjectManager.World);
                        
                        var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<SceneGroup>();
                        foreach (var id in SaveManager.CurrentLevel.BuildObjects.Keys)
                        {
                            MelonLogger.Msg(id);
                            foreach (var data in SaveManager.CurrentLevel.BuildObjects[id])
                            {
                                if (ObjectManager.BuildObjectsData.TryGetValue(id, out var bObj ))
                                {
                                    GameObject obj = Object.Instantiate(bObj.GameObject, BuildObjectData.Vector3Save.RevertToVector3(data.Pos), Quaternion.Euler(BuildObjectData.Vector3Save.RevertToVector3(data.Rot)), ObjectManager.World.transform);
                                    var buildObject = obj.AddComponent<BuildObject>();
                                    buildObject.SceneGroup = findObjectsOfTypeAll.FirstOrDefault(x => x.ReferenceId.Equals(data.SceneGroup));
                                    buildObject.ID = bObj;
                                    obj.transform.localScale = BuildObjectData.Vector3Save.RevertToVector3(data.Scale);
                                    obj.SetActive(true);
                                   
                                    ObjectManager.AddObject(id, obj);
                                }
                                else
                                {
                                    MelonLogger.Msg($"[SRLE] Can't find the gameobject with id: {id}");
                                }
                        
                                ToolbarUI.Instance.UpdateStatus();
                            }
                        }
                        ToolbarUI.Instance.UpdateStatus();
                        LevelManager.IsLoading = false;
                    })
                });
            }
        }
    }
    
}