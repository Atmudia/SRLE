using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MelonLoader;
using SRLE.Components;
using SRLE.Models;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE;



public static class ObjectManager
{
    public static Dictionary<uint, IdClass> BuildObjectsData = new Dictionary<uint, IdClass>();
    public static Dictionary<string, List<uint>> BuildCategories = new Dictionary<string, List<uint>>();
    public static GameObject CachedGameObjects;
    private static Dictionary<uint, List<Action<GameObject>>> ObjectRequests = new Dictionary<uint, List<Action<GameObject>>>();
    public static GameObject World;
    

    internal static string[] ListedAssetBundles;
    public static Dictionary<uint, Il2CppSystem.Collections.Generic.List<GameObject>> BuildObjects = new Dictionary<uint, Il2CppSystem.Collections.Generic.List<GameObject>>();

    static ObjectManager()
    {
        Melon<EntryPoint>.Logger.Msg("Loading Build Objects");
        LoadBuildObjects();
        ListedAssetBundles = Directory.GetFiles(
            Path.Combine(Application.streamingAssetsPath, "aa", "StandaloneWindows64"), "*.bundle",
            SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();
    }
    public static void LoadBuildObjects()
    {
        string json;
        // json = File.ReadAllText("BuildObjects.json");
        
        using (StreamReader streamReader = new StreamReader(Melon<EntryPoint>.Instance.MelonAssembly.Assembly.GetManifestResourceStream("SRLE.BuildObjects.json") ?? throw new NullReferenceException("Build Objects are null. Please report this bug")))
        {
            json = streamReader.ReadToEnd();
        }
        List<SRLE.Models.IdCategoryData> buildCategories = JsonSerializer.Deserialize<List<IdCategoryData>>(json);
        foreach (var category in buildCategories)
        {
            List<uint> objectIDs = [];
            foreach (var buildObject in category.Objects)
            {
                objectIDs.Add(buildObject.Id);
                BuildObjectsData.Add(buildObject.Id, buildObject);
            }
            BuildCategories.Add(category.CategoryName, objectIDs);
        }

        BuildCategories["Favorites"] = JsonSerializer.Deserialize<List<uint>>(File.ReadAllText(Path.Combine(SaveManager.DataPath, "favorites.txt")));
    }
    public static void RequestObject(uint id, Action<GameObject> callback)
    {
        if (BuildObjectsData.TryGetValue(id, out var obj))
        {
            callback(obj.GameObject);
        }
        else
        {
            if (ObjectRequests.ContainsKey(id))
            {
                ObjectRequests[id].Add(callback);
            }
            else
            {
                ObjectRequests.Add(id, new List<Action<GameObject>>() { callback });
            }
        }
    }
    public static void UpdateRequests()
    {
        if (ObjectRequests.Count <= 0) return;

        var request = ObjectRequests.First();

        if (BuildObjectsData.TryGetValue(request.Key, out IdClass data))
        {
            GameObject worldObject = data.GameObject;
            if (worldObject != null)
            {
                // var buildObject = LoadWorldObject(data, worldObject);

                foreach (var callback in request.Value)
                {
                    callback(worldObject);
                }
            }
            else
            {
                UnityEngine.Debug.Log($"[SRLE] Could not load {data.Id} because path {data.Path} does not exist");
                foreach (var callback in request.Value)
                {
                    callback(null);
                }
            }
        }
        else
        {
            Debug.Log("[SRLE] Failed to load object ID " + request.Key);
            foreach (var callback in request.Value)
            {
                callback(null);
            }
        }

        ObjectRequests.Remove(request.Key);
    }

    internal static void AddObject(uint id, GameObject obj)
    {
        if (BuildObjects.ContainsKey(id))
        {
            BuildObjects[id].Add(obj);
        }
        else
        {
            var value = new Il2CppSystem.Collections.Generic.List<GameObject>();
            value.Add(obj);
            BuildObjects.Add(id, value);
        }
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
        {
            ToolbarUI.Instance.UpdateStatus();
        }
    }
    public static void  RemoveObject(GameObject obj)
    {
        //TODO Add Undo and Redo Here
        if (ObjectManager.GetBuildObject(obj, out var buildObj))
        {
            if (BuildObjects.ContainsKey(buildObj.ID.Id))
            {
                BuildObjects[buildObj.ID.Id].Remove(obj);
            }
            SRLECamera.Instance.transformGizmo.ClearTargets();
            Object.Destroy(obj);
            InspectorUI.Instance.SetActive(false);
        }
        ToolbarUI.Instance.UpdateStatus();
    }
    public static void RemoveObject(uint id, GameObject obj)
    {
        if (BuildObjects.ContainsKey(id))
        {
            BuildObjects[id].Remove(obj);
        }
        ToolbarUI.Instance.UpdateStatus();
    }
    

    public static GameObject GetObjectById(uint id)
    {
        if (BuildObjectsData.TryGetValue(id, out var data))
        {
            return data.GameObject;
        }
        Melon<EntryPoint>.Logger.Warning($"[SRLE] Can't load object by id: {id}");
        return null;
    }
    public static bool GetBuildObject(GameObject obj, out BuildObject buildObject)
    {
        Transform target = obj.transform;
        buildObject = target.GetComponent<BuildObject>();
        if (!buildObject)
            buildObject = target.GetComponentInParent<BuildObject>();
        if (!buildObject)
            buildObject = target.GetComponentInChildren<BuildObject>();

        return buildObject != null;
    }

}