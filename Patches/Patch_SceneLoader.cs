using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.Loading;
using MelonLoader;
using SR2E.Utils;
using SRLE.Components;
using SRLE.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SRLE.Patches;

[HarmonyPatch(typeof(SceneLoader), nameof(SceneLoader.LoadSceneGroup))]
public static class Patch_SceneLoader
{
    static List<string> ScenesToLoad = new()
    {
        "SceneGroup.ConservatoryFields",
        "SceneGroup.LuminousStrand",
        "SceneGroup.RumblingGorge",
        "SceneGroup.PowderfallBluffs",
        "SceneGroup.Labyrinth"
    };
    public static bool IsLoadingAllObjects = false;
    public static bool HasLoadedAllObjects = false;
    private static SceneGroup toLoadAtEnd = null;
    private static Vector3 toPositonAtEnd = Vector3.zero;
    private static Vector3 toRotationAtEnd = Vector3.zero;
    
    private static string tempSG = null;
    
    public static void LoadObjectsAndGoToSceneGroup(SceneGroup sceneGroup,Vector3 position, Vector3 rotation)
    {
        if (IsLoadingAllObjects) return;
        if (!sceneGroup.IsGameplay) return;
        try
        {
            toLoadAtEnd = sceneGroup;
            toPositonAtEnd = position;
            toRotationAtEnd = rotation;
            if (!HasLoadedAllObjects)
            {
                
                ObjectManager.CachedGameObjects = new GameObject(nameof(ObjectManager.CachedGameObjects))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                Object.DontDestroyOnLoad(ObjectManager.CachedGameObjects);
                File.Delete(SaveManager.BuildObjectsPath);
                currentlyLoadingSceneGroup = 0;
                IsLoadingAllObjects = true;
                tempSG = SystemContext.Instance.SceneLoader.CurrentSceneGroup.ReferenceId;
                LocationBookmarksUtil.GoToLocationPlayer(GetSceneGroup(ScenesToLoad[currentlyLoadingSceneGroup]), new Vector3(999,99999f,999), new Vector3(0,0,0));
            }
            else
            {
                FinalizeSceneLoad();
            }
        }
        catch { }
    }

    static SceneGroup GetSceneGroup(string ReferenceID) => SystemContext.Instance.SceneLoader.SceneGroupList.items.ToNetList().FirstOrDefault(x => x.ReferenceId.Contains(ReferenceID));

    
    static int currentlyLoadingSceneGroup = 0;

    static void FinalizeSceneLoad()
    {
        EntryPoint.InfoText = "Loading level!";
        ObjectManager.LoadObjectManager();
        SRSingleton<SystemContext>.Instance.SceneLoader.LoadSceneGroup(
            toLoadAtEnd, new SceneLoadingParameters()
            {
                TeleportPlayer = false,
                OnSceneGroupLoadedPhase2 = new Action<Il2CppSystem.Action<SceneLoadErrorData>>(_ =>
                {
                    ObjectManager.ReconnectPrefabs();
                    SRSingleton<SceneContext>.Instance.Player.GetComponent<SRCharacterController>().Position =
                        toPositonAtEnd;
                    UIInitializer.Initialize();
                    ObjectManager.World = new GameObject("SRLEWorld");
                    ObjectManager.World.hideFlags |= HideFlags.HideAndDontSave;
                    Object.DontDestroyOnLoad(ObjectManager.World);


                    var gameplaySceneGroups = SystemContext.Instance.SceneLoader._sceneGroupList._gameplaySceneGroups;
                    foreach (var id in SaveManager.CurrentLevel.BuildObjects.Keys)
                    {
                        MelonLogger.Msg(id);
                        foreach (var data in SaveManager.CurrentLevel.BuildObjects[id])
                        {
                            if (ObjectManager.BuildObjectsData.TryGetValue(id, out var bObj))
                            {
                                GameObject obj = Object.Instantiate(bObj.GameObject,
                                    BuildObjectData.Vector3Save.RevertToVector3(data.Pos),
                                    Quaternion.Euler(BuildObjectData.Vector3Save.RevertToVector3(data.Rot)),
                                    ObjectManager.World.transform);
                                var buildObject = obj.AddComponent<BuildObject>();
                                buildObject.SceneGroup =
                                    gameplaySceneGroups.FirstOrDefault(x => x.ReferenceId.Equals(data.SceneGroup));
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
                    EntryPoint.InfoText = "";
                })
            });
    }

    static IEnumerable<Scene> GetAllScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; ++index)
            yield return SceneManager.GetSceneAt(index);
    }

    public static void OnSceneLoaded()
    {
        if (IsLoadingAllObjects)
        {
            foreach (var view in UnityEUtil.GetAll<LoadingScreenView>())
            {
                if (view.enabled && view.gameObject.gameObject.activeSelf)
                {
                    ActionsEUtil.ExecuteInTicks(()=> OnSceneLoaded(),2);
                    return;
                }
            }
            if (tempSG != SystemContext.Instance.SceneLoader.CurrentSceneGroup.ReferenceId)
            {
                
                currentlyLoadingSceneGroup++;
                if (currentlyLoadingSceneGroup >= ScenesToLoad.Count)
                {
                    IsLoadingAllObjects = false;
                    HasLoadedAllObjects = true;
                    FinalizeSceneLoad();
                    return;
                }

                tempSG = SystemContext.Instance.SceneLoader.CurrentSceneGroup.ReferenceId;
                try
                {
                    LocationBookmarksUtil.GoToLocationPlayer(GetSceneGroup(ScenesToLoad[currentlyLoadingSceneGroup]), new Vector3(999,99999f,999), new Vector3(0,0,0));
                } catch { }
                
            }
            else ActionsEUtil.ExecuteInTicks(()=> OnSceneLoaded(),2);
        }
    }
    public static void Prefix(SceneLoader __instance,ref SceneGroup sceneGroup, SceneLoadingParameters parameters)
    {
        if (!IsLoadingAllObjects) return;
        
        EntryPoint.InfoText="Loading objects of "+sceneGroup.ReferenceId+"!";
        
        parameters.TeleportPlayer = true;
        parameters.OnSceneGroupLoadedPhase2 += new System.Action<Il2CppSystem.Action<SceneLoadErrorData>>(action =>
        {
            SRLEConverter.ConvertToBuildObjects();

        });
    }
    
}

