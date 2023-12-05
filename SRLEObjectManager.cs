using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using Newtonsoft.Json;
using SRLE.Components;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE;

public static class SRLEObjectManager
{
    public static GameObject CachedGameObjects;
    public static AssetBundle SRLEAssetBundle;
    public static AssetBundle SRLEScenesAssetBundle;
    public static Shader[] Shaders;
    public static string EmptyLevelPath;
    
    public static Dictionary<uint, BuildObjects.IdClass> BuildObjectsData;
    public static Dictionary<string, List<uint>> BuildCategories;

    public static void LoadBuildObjects()
    {
        BuildObjectsData = new Dictionary<uint, BuildObjects.IdClass>();
        BuildCategories = new Dictionary<string, List<uint>>();
        
        

        var buildCategories = JsonConvert.DeserializeObject<List<BuildObjects.Category>>(File.ReadAllText(SRLESaveManager.BuildObjectsPath));
        foreach (var category in buildCategories)
        {
            List<uint> objectIDs = new List<uint>();
            foreach (var buildobject in category.Objects)
            {
                objectIDs.Add(buildobject.Id);
                BuildObjectsData.Add(buildobject.Id, buildobject);
            }
            BuildCategories.Add(category.CategoryName, objectIDs);
        }
        BuildCategories["Favorites"] = JsonConvert.DeserializeObject<List<uint>>(File.ReadAllText("D:\\SteamLibrary\\steamapps\\common\\Slime Rancher 2\\SRLE\\favorites.txt"));

        SRLEAssetBundle = UnityEngine.AssetBundle.LoadFromFile(@"C:\Users\komik\SRLEGizmo\Assets\AssetBundles\srle");
        SRLEScenesAssetBundle = AssetBundle.LoadFromFile(@"C:\Users\komik\SRLEGizmo\Assets\AssetBundles\srlescene");
        EmptyLevelPath = SRLEScenesAssetBundle.GetAllScenePaths()[0];
        Shaders = SRLEAssetBundle.LoadAllAssets(Il2CppType.Of<Shader>()).Select(delegate(Object o)
        {
            var shader = o.Cast<Shader>();
            shader.hideFlags |= HideFlags.HideAndDontSave;
            return shader;
        }).ToArray();
        
    }

    public static void SpawnObject(uint id, BuildObject buildObject = null, bool async = true)
    {
        var findObject = FindObject(id);
        if (findObject == null)
            return;
        if (async)
        {
            MelonCoroutines.Start(SpawnObjectWithAsync(findObject,  buildObject));
            return;
        }

        var gameObject = UnityEngine.Object.Instantiate(findObject.GameObject, SRLEManager.World.transform, true);
        
        gameObject.transform.position = buildObject == null ? SRLECamera.Instance.transform.position : buildObject.Pos.ToVector3();
        gameObject.transform.rotation = buildObject == null ? SRLECamera.Instance.transform.rotation : Quaternion.Euler(buildObject.Rot.ToVector3());
        gameObject.transform.localScale = buildObject == null ? Vector3.zero : buildObject.Scale.ToVector3();
        gameObject.SetActive(true);
        gameObject.AddComponent<BuildObjectId>().IdClass = findObject;
        
        if (buildObject == null)
        {
            /*
            if (!SRLESaveManager.CurrentLevel.BuildObjects.TryGetValue(id, out var value))
            {
                var buildObjects = new List<BuildObject>();
                value = buildObjects;
                SRLESaveManager.CurrentLevel.BuildObjects.Add(id, value);
            }
            value.Add(new BuildObject()
            {
                Pos = gameObject.transform.position.ToVector3Save(),
                Rot = gameObject.transform.localEulerAngles.ToVector3Save(),
                Scale = gameObject.transform.localScale.ToVector3Save(),
                Properties = new Dictionary<string, string>()
            });
            */
        }
        
                    
        MelonLogger.Msg($"Spawned with: {gameObject.name} ");
    }
    public static IEnumerator<GameObject> SpawnObjectWithAsync(BuildObjects.IdClass id, BuildObject buildObject)
    {
        
        Vector3 spawnPosition = SRLECamera.Instance.transform.position + SRLECamera.Instance.transform.forward * 10 ;
        Quaternion spawnRotation = SRLECamera.Instance.transform.rotation;
        
        var gameObject = UnityEngine.Object.Instantiate(id.GameObject, SRLEManager.World.transform, true);
        gameObject.transform.position = buildObject == null ? spawnPosition : buildObject.Pos.ToVector3();
        gameObject.transform.rotation = buildObject == null ? spawnRotation : Quaternion.Euler(buildObject.Rot.ToVector3());
        gameObject.transform.localScale = buildObject == null ? id.GameObject.transform.localScale : buildObject.Scale.ToVector3();
        gameObject.SetActive(true);
        gameObject.AddComponent<BuildObjectId>().IdClass = id;
        
        
        if (buildObject == null)
        {
            /*
            if (!SRLESaveManager.CurrentLevel.BuildObjects.TryGetValue(id.Id, out var value))
            {
                var buildObjects = new List<BuildObject>();
                value = buildObjects;
                SRLESaveManager.CurrentLevel.BuildObjects.Add(id.Id, value);
            }
            value.Add(new BuildObject()
            {
                Pos = gameObject.transform.position.ToVector3Save(),
                Rot = gameObject.transform.localEulerAngles.ToVector3Save(),
                Scale = gameObject.transform.localScale.ToVector3Save(),
                Properties = new Dictionary<string, string>()
            });
            */
        }
        
        MelonLogger.Msg($"Spawned with async method: {gameObject.name} ");
        yield return gameObject;

    }
    public static void RequestObject(uint id, Action<GameObject> callback)
    {
        var findObject = FindObject(id);
        callback(findObject.GameObject);
    }

    
    public static BuildObjects.IdClass FindObject(uint id)
    {
        var idClass = BuildObjectsData[id];
        if (idClass == null)
        {
            MelonLogger.Warning($"Can't find object with id: {id}");
            return null;
        }

        return idClass;
    }

}