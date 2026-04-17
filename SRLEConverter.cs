using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SRLE.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRLE
{
    public static class SRLEConverter
    {
        public static List<IdCategoryData> CategoryDatas;

        private static readonly HashSet<string> included = new HashSet<string>
        {
            "Main Nav", "Cliffs", "Mountains", "Solid Filler", "Rocks", "Flora", "Grass",
            "Deco", "Constructs", "Slimes", "Lights", "Water",
            "Colliders", "Loot", "Audio", "Build Sites", "Roots", "Upgrades",
            "Ranch Features", "Drone Network"
        };

        internal static IEnumerable<Scene> GetAllScenes()
        {
            for (int index = 0; index < SceneManager.sceneCount; ++index)
                yield return SceneManager.GetSceneAt(index);
        }

        public static void ConvertToBuildObjects()
        {
            HashSet<int> hashcodes = new HashSet<int>();
            Dictionary<GameObject, int> objectHashMap = new Dictionary<GameObject, int>();

            foreach (var scene in GetAllScenes().Where(x => !x.name.EndsWith("Core")))
            {
                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    foreach (var cellDirector in rootGameObject.GetComponentsInChildren<CellDirector>())
                    {
                        if (cellDirector.transform.parent?.name == "PrefabParent")
                            continue;

                        var sector = cellDirector.transform.Find("Sector");
                        if (sector == null)
                            continue;

                        foreach (Transform elementsTransform in sector)
                        {
                            if (!included.Contains(elementsTransform.gameObject.name))
                                continue;

                            foreach (Transform elementTransform in elementsTransform)
                            {
                                int hash = ComputeHash(elementTransform);
                                if (hash == 0 || !hashcodes.Add(hash))
                                    continue;

                                objectHashMap.Add(elementTransform.gameObject, hash);
                            }
                        }
                    }
                }

                uint id = 1;
                List<IdCategoryData> categories = new List<IdCategoryData>();

                foreach (var element in objectHashMap)
                {
                    var path = element.Key.GetFullPath();
                    var parts = path.Split('/');
                    var categoryName = parts.Length > 3 ? parts[3] : "None";
                    var idClass = new IdClass { Id = id, Name = element.Key.name, Path = path, Zone = element.Key.scene.name, HashCode = element.Value };
                    GetOrAddCategory(categoryName, categories).Objects.Add(idClass);
                    id++;
                }

                File.WriteAllText(Path.Combine(SaveManager.DataPath, "BuildObjects.json"), JsonConvert.SerializeObject(categories, Formatting.Indented));
                CategoryDatas = categories;
            }
        }

        private static int ComputeHash(Transform t)
        {
            if (t.childCount == 0)
            {
                var meshFilter = t.GetComponent<MeshFilter>();
                var renderer = t.GetComponent<Renderer>();
                if (meshFilter == null || renderer == null)
                    return 0;

                string meshName = meshFilter.sharedMesh?.name ?? string.Empty;
                string materialName = renderer.material?.name ?? string.Empty;
                return (meshName + materialName).GetHashCode();
            }
            else
            {
                string text = string.Concat(t.GetComponentsInChildren<MeshFilter>()
                    .Where(mf => mf.sharedMesh)
                    .Select(mf => mf.sharedMesh.name));
                text += string.Concat(t.GetComponentsInChildren<Renderer>()
                    .Where(r => r.material)
                    .Select(r => r.material.name));

                return text.Length == 0 ? 0 : text.GetHashCode();
            }
        }

        private static IdCategoryData GetOrAddCategory(string categoryName, List<IdCategoryData> categories)
        {
            var category = categories.FirstOrDefault(x => x.CategoryName == categoryName);
            if (category != null)
                return category;

            var newCategory = new IdCategoryData { CategoryName = categoryName, Objects = new List<IdClass>() };
            categories.Add(newCategory);
            return newCategory;
        }
    }
}
