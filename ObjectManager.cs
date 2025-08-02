using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SRLE.Components;
// using System.Text.Json;
using SRLE.Models;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE
{
    public static class ObjectManager
    {
        public static Dictionary<uint, IdClass> BuildObjectsData = new Dictionary<uint, IdClass>();
        public static Dictionary<string, List<uint>> BuildCategories = new Dictionary<string, List<uint>>();
        public static GameObject CachedGameObjects;
        private static Dictionary<uint, List<Action<IdClass>>> ObjectRequests = new Dictionary<uint, List<Action<IdClass>>>();
        public static GameObject World;
    

        public static Dictionary<uint, List<GameObject>> BuildObjects = new Dictionary<uint, List<GameObject>>();

        static ObjectManager()
        {
            EntryPoint.ConsoleInstance.Log("Loading Build Objects");
            LoadBuildObjects();
        }
        public static void LoadBuildObjects()
        {
            // return;
            string json;
            json = File.ReadAllText("BuildObjects.json");
        
            // using (StreamReader streamReader = new StreamReader(Melon<EntryPoint>.Instance.MelonAssembly.Assembly.GetManifestResourceStream("SRLE.BuildObjects.json") ?? throw new NullReferenceException("Build Objects are null. Please report this bug")))
            // {
            //     json = streamReader.ReadToEnd();
            // }
            List<SRLE.Models.IdCategoryData> buildCategories = JsonConvert.DeserializeObject<List<IdCategoryData>>(json);
            foreach (var category in buildCategories)
            {
                List<uint> objectIDs = new List<uint>();
                foreach (var buildObject in category.Objects)
                {
                    objectIDs.Add(buildObject.Id);
                    BuildObjectsData.Add(buildObject.Id, buildObject);
                }
                BuildCategories.Add(category.CategoryName, objectIDs);
            }

            BuildCategories["Favorites"] = JsonConvert.DeserializeObject<List<uint>>(File.ReadAllText(Path.Combine(SaveManager.DataPath, "favorites.txt")));
        }
        public static void RequestObject(uint id, Action<IdClass> callback)
        {
            if (BuildObjectsData.TryGetValue(id, out var obj))
            {
                callback(obj.GameObject == null ? null : obj);
            }
            else
            {
                if (ObjectRequests.ContainsKey(id))
                {
                    ObjectRequests[id].Add(callback);
                }
                else
                {
                    ObjectRequests.Add(id, new List<Action<IdClass>>() { callback });
                }
            }
        }

        public static bool TryGetObject(int objectHash, string name, string path, out IdClass idClass)
        {
            if (GetObjectByHashCode(objectHash, out idClass))
            {
                return true;
            }
            if (GetObjectByName(name, out idClass))
            {
                return true;
            }
            if (GetObjectByPath(path, out idClass))
            {
                return true;
            }
            EntryPoint.ConsoleInstance.Log($"Can't find object via all methods: {objectHash}, {name}, {path}");
            return false;
            
        }
        

        public static bool GetObjectByHashCode(int objectHash, out IdClass objectClass)
        {
            objectClass = BuildObjectsData.Values.FirstOrDefault(x => x.HashCode == objectHash);
            return objectClass != null;
        }
        public static bool GetObjectByName(string name, out IdClass objectClass)
        {
            objectClass = BuildObjectsData.Values.FirstOrDefault(x => x.Name == name);
            return objectClass != null;
        }
        public static bool GetObjectByPath(string path, out IdClass objectClass)
        {
            objectClass = BuildObjectsData.Values.FirstOrDefault(x => x.Path == path);
            return objectClass != null;
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
                        callback(data);
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
                var value = new System.Collections.Generic.List<GameObject>();
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
                Object.DestroyImmediate(obj);
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
            // ToolbarUI.Instance.UpdateStatus();
        }
    

        public static GameObject GetObjectById(uint id)
        {
            if (BuildObjectsData.TryGetValue(id, out var data))
            {
                return data.GameObject;
            }
            EntryPoint.ConsoleInstance.LogWarning($"[SRLE] Can't load object by id: {id}");
            return null;
        }
        public static bool GetBuildObject(GameObject obj, out BuildObject buildObject)
        {
            buildObject = obj.GetComponent<BuildObject>() 
                          ?? obj.GetComponentInParent<BuildObject>() 
                          ?? obj.GetComponentInChildren<BuildObject>();
            return buildObject;
        }


    }
}