using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.Models;
using SRLE.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE
{
    public static class ObjectManager
    {
        public static Dictionary<uint, IdClass> BuildObjectsData = new Dictionary<uint, IdClass>();
        public static Dictionary<string, List<uint>> BuildCategories = new Dictionary<string, List<uint>>();
        public static Dictionary<uint, List<GameObject>> BuildObjects = new Dictionary<uint, List<GameObject>>();
        public static GameObject CachedGameObjects;
        public static GameObject World;

        private static Dictionary<uint, List<Action<IdClass>>> ObjectRequests = new Dictionary<uint, List<Action<IdClass>>>();

        private static readonly HashSet<string> m_DefaultRemovedScripts = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ActivateOnProgressRange",
            "DeactivateOnGameMode",
            "DeactivateOnDLCDisabled",
            "PuzzleTeleportLock"
        };
        static ObjectManager()
        {
            EntryPoint.ConsoleInstance.Log("Loading Build Objects");

            CachedGameObjects = new GameObject(nameof(CachedGameObjects)) { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(CachedGameObjects);

            World = new GameObject("SRLEWorld") { hideFlags = HideFlags.HideAndDontSave };
            Object.DontDestroyOnLoad(World);

            LoadBuildObjects();
            CacheSceneObjects();
        }

        private static void CacheSceneObjects()
        {
            var bundleCachePath = Path.Combine(SaveManager.DataPath, "worldGenerated.bundle");
            byte[] bundleBytes;
            if (File.Exists(bundleCachePath))
            {
                bundleBytes = File.ReadAllBytes(bundleCachePath);
            }
            else
            {
                bundleBytes = SceneBundleBuilder.CreateSceneLoaderBundle("worldGenerated");
                File.WriteAllBytes(bundleCachePath, bundleBytes);
            }

            var bundle = AssetBundle.LoadFromMemory(bundleBytes);
            var rootObjects = bundle.LoadAllAssets<GameObject>()
                .Where(x => x.name.StartsWith("zone"))
                .ToDictionary(x => x.name, x => x);

            foreach (var idClass in BuildObjectsData.Values)
            {
                var source = FindInScene(rootObjects, idClass);
                if (source == null)
                {
                    EntryPoint.ConsoleInstance.LogWarning($"[SRLE] Object {idClass.Id} not found at path '{idClass.Path}'");
                    continue;
                }

                var wasActive = source.activeInHierarchy;
                source.SetActive(false);

                var cached = Object.Instantiate(source, Vector3.zero, source.transform.rotation, CachedGameObjects.transform);
                cached.name = $"{idClass.Id} {source.name}";

                foreach (var script in cached.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (m_DefaultRemovedScripts.Contains(script.GetType().Name))
                        Object.Destroy(script);
                }

                var destination = cached.GetComponentInChildren<TeleportDestination>(true);
                if (destination != null)
                    destination.teleportDestinationName = "NotSet";

                var teleportSource = cached.GetComponentInChildren<TeleportSource>(true);
                if (teleportSource != null)
                {
                    teleportSource.activated = false;
                    teleportSource.activationBlocker = null;
                    teleportSource.waitForExternalActivation = false;
                    teleportSource.activationProgress = ProgressDirector.ProgressType.NONE;
                    teleportSource.blockingGenerator = null;
                    teleportSource.destinationSetName = "NotSet";
                }

                var journal = cached.GetComponentInChildren<JournalEntry>();
                if (journal != null)
                    journal.ensureProgress = Array.Empty<ProgressDirector.ProgressType>();

                source.SetActive(wasActive);
                idClass.gameObject = cached;
            }
            bundle.Unload(true);
        }

        private static GameObject FindInScene(Dictionary<string, GameObject> roots, IdClass idClass)
        {
            var slashIndex = idClass.Path.IndexOf('/');
            if (slashIndex == -1)
                return roots.TryGetValue(idClass.Path, out var root) ? root : null;

            var rootName = idClass.Path.Substring(0, slashIndex);
            var relativePath = idClass.Path.Substring(slashIndex + 1);
            if (!roots.TryGetValue(rootName, out var rootObj))
                return null;

            var found = rootObj.transform.Find(relativePath);
            return found != null ? found.gameObject : null;
        }

        public static void LoadBuildObjects()
        {
            var json = File.ReadAllText(Path.Combine(SaveManager.DataPath, "BuildObjects.json"));
            var buildCategories = JsonConvert.DeserializeObject<List<IdCategoryData>>(json);

            foreach (var category in buildCategories)
            {
                var objectIDs = new List<uint>();
                foreach (var buildObject in category.Objects)
                {
                    objectIDs.Add(buildObject.Id);
                    BuildObjectsData.Add(buildObject.Id, buildObject);
                }
                BuildCategories.Add(category.CategoryName, objectIDs);
            }

            BuildCategories["Favorites"] = JsonConvert.DeserializeObject<List<uint>>(
                File.ReadAllText(Path.Combine(SaveManager.DataPath, "favorites.txt")));
        }

        public static void RequestObject(uint id, Action<IdClass> callback)
        {
            if (BuildObjectsData.TryGetValue(id, out var obj))
            {
                callback(obj.gameObject == null ? null : obj);
                return;
            }

            if (!ObjectRequests.TryGetValue(id, out var list))
            {
                list = new List<Action<IdClass>>();
                ObjectRequests.Add(id, list);
            }
            list.Add(callback);
        }

        public static void UpdateRequests()
        {
            if (ObjectRequests.Count <= 0) return;

            var request = ObjectRequests.First();
            ObjectRequests.Remove(request.Key);

            if (BuildObjectsData.TryGetValue(request.Key, out var data) && data.gameObject != null)
            {
                foreach (var callback in request.Value)
                    callback(data);
            }
            else
            {
                EntryPoint.ConsoleInstance.LogWarning($"[SRLE] Failed to load object ID {request.Key}");
                foreach (var callback in request.Value)
                    callback(null);
            }
        }

        public static bool TryGetObject(int objectHash, string name, string path, out IdClass idClass)
        {
            if (GetObjectByHashCode(objectHash, out idClass)) return true;
            if (GetObjectByName(name, out idClass)) return true;
            if (GetObjectByPath(path, out idClass)) return true;

            EntryPoint.ConsoleInstance.LogWarning($"[SRLE] Can't find object via any method: hash={objectHash}, name={name}, path={path}");
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

        public static GameObject GetObjectById(uint id)
        {
            if (BuildObjectsData.TryGetValue(id, out var data))
                return data.gameObject;

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

        internal static void AddObject(uint id, GameObject obj)
        {
            if (!BuildObjects.TryGetValue(id, out var list))
            {
                list = new List<GameObject>();
                BuildObjects.Add(id, list);
            }
            list.Add(obj);

            if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
                ToolbarUI.Instance.UpdateStatus();
        }

        public static void RemoveObject(GameObject obj)
        {
            //TODO: Add Undo/Redo
            if (!GetBuildObject(obj, out var buildObj)) return;

            if (BuildObjects.TryGetValue(buildObj.ID.Id, out var list))
                list.Remove(obj);

            SRLECamera.Instance.transformGizmo.ClearTargets();
            Object.DestroyImmediate(obj);
            InspectorUI.Instance.SetActive(false);
            ToolbarUI.Instance.UpdateStatus();
        }

        public static void RemoveObject(uint id, GameObject obj)
        {
            if (BuildObjects.TryGetValue(id, out var list))
                list.Remove(obj);
        }

        /// <summary>
        /// Registers a modded object so it appears in the build hierarchy and can be placed/saved in levels.
        /// Call this before the HierarchyUI is initialized (e.g. on mod load).
        /// </summary>
        /// <param name="modId">Unique identifier for your mod (e.g. "MyMod"). Used for stable ID generation and level dependencies.</param>
        /// <param name="categoryName">Category to place the object under in the hierarchy. Created if it doesn't exist.</param>
        /// <param name="objectName">Display name of the object.</param>
        /// <param name="prefab">The GameObject prefab to use when spawning this object.</param>
        public static void RegisterObject(string modId, string categoryName, string objectName, GameObject prefab)
        {
            uint id = GenerateModId(modId, objectName);

            if (BuildObjectsData.ContainsKey(id))
            {
                EntryPoint.ConsoleInstance.LogWarning($"[SRLE] Mod object ID collision for '{modId}/{objectName}' (id={id}), skipping");
                return;
            }

            var idClass = new ModdedIdClass
            {
                ModId = modId,
                Name = objectName,
                Id = id,
                Path = $"mod:{modId}/{objectName}",
                Zone = "Mod",
                HashCode = prefab.name.GetHashCode(),
                gameObject = prefab
            };

            BuildObjectsData[id] = idClass;

            if (!BuildCategories.TryGetValue(categoryName, out var list))
            {
                list = new List<uint>();
                BuildCategories[categoryName] = list;
            }
            list.Add(id);
        }

        private static uint GenerateModId(string modId, string objectName)
        {
            // FNV-1a hash for deterministic IDs; high bit set to avoid vanilla ID range
            unchecked
            {
                uint hash = 2166136261u;
                foreach (char c in modId + "/" + objectName)
                    hash = (hash ^ c) * 16777619u;
                return hash | 0x80000000u;
            }
        }
    }
}
