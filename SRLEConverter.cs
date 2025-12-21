using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MelonLoader;
using SRLE.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRLE;

public static class SRLEConverter
{
    private static List<string> included = new()
    {
        "Main Nav", "Cliffs", "Mountains", "Solid Filler", "Rocks", "Flora", "Grass",
        "Deco", "Constructs", "Resources", "Slimes", "FX", "Lights", "Water",
        "Colliders", "Loot", "Audio", "Build Sites", "Roots", "Upgrades",
        "Ranch Features", "Drone Network"
    };
    public static string GetFullPath(this GameObject obj)
    {
        string str = obj.name;
        for (Transform parent = obj.transform.parent;
             parent != null;
             parent = parent.parent)
        {
            str = parent.gameObject.name + "/" + str;
        }

        return str;
    }


    internal static IEnumerable<Scene> GetAllScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; ++index)
            yield return SceneManager.GetSceneAt(index);
    }
    static List<IdCategoryData> allCategories = new List<IdCategoryData>();
    static uint aa = 1;
    public static void ConvertToBuildObjects()
    {
        List<int> hashcodes = new List<int>();
        Dictionary<GameObject, int> listOfCategory = new Dictionary<GameObject, int>();
        var currSceneGroup = SystemContext.Instance.SceneLoader.CurrentSceneGroup.ReferenceId;
        
        foreach (var scene in GetAllScenes().Where(x => !x.name.EndsWith("Core")))
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var cellDirector in rootGameObject.GetComponentsInChildren<CellDirector>())
                {
                    if (cellDirector.transform.parent)
                    {
                        if (cellDirector.transform.parent.name == "PrefabParent")
                        {
                            continue;
                        }
                    }

                    var sector = cellDirector.transform.Find("Sector");
                    if (sector == null)
                        continue;
                    foreach (var elements in sector)
                    {
                        var elementsTransform = elements.Cast<Transform>();
                        if (!included.Exists(s => s.Equals(elementsTransform.gameObject.name)))
                            continue;

                        foreach (var element in elementsTransform)
                        {
                            var elementTransform = element.Cast<Transform>();
                            bool ignore = true;
                            int num2 = -1;
                            if (elementTransform.childCount <= 0 && ignore)
                            {
                                MeshFilter meshFilter = elementTransform.GetComponent<MeshFilter>();
                                Renderer renderer = elementTransform.GetComponent<Renderer>();
                                if (renderer && meshFilter)
                                {
                                    string meshName = meshFilter.sharedMesh?.name ?? string.Empty;
                                    string materialName = renderer.material?.name ?? string.Empty;

                                    num2 = (meshName + materialName).GetHashCode();

                                    if (hashcodes.Contains(num2))
                                    {
                                        continue;
                                    }

                                    hashcodes.Add(num2);
                                }
                            }
                            else if (ignore)
                            {
                                string text = "";
                                MeshFilter[] meshFilters = elementTransform.GetComponentsInChildren<MeshFilter>();
                                Renderer[] renderers = elementTransform.GetComponentsInChildren<Renderer>();
                                foreach (var meshFilter in meshFilters)
                                {
                                    if (meshFilter.sharedMesh)
                                    {
                                        text += meshFilter.sharedMesh.name;
                                    }
                                }
                                foreach (var renderer in renderers)
                                {
                                    if (renderer.material)
                                    {
                                        text += renderer.material.name;
                                    }
                                }
                                if (text == "")
                                {
                                    continue;
                                }
                                num2 = text.GetHashCode();
                                if (hashcodes.Contains(num2))
                                {
                                    continue;
                                }
                                hashcodes.Add(num2);
                            }
                            if (num2 != -1 && ignore)
                            {
                                listOfCategory.Add(elementTransform.gameObject, num2);   
                            }
                        }
                    }
                }
            }
            foreach (var element in listOfCategory)
            {
                var path = element.Key.GetFullPath();
                var strings = path.Split('/', '/');
                var category = InBounds(3, strings) ? strings[3] : "None";
                var sceneName = element.Key.scene.name;
                var idClass = new IdClass {Id = aa, Name = element.Key.name, Path = path, Scene = sceneName, SceneGroup = currSceneGroup, HashCode = element.Value};

                var key = currSceneGroup + sceneName + path;
                if (FindCategory(category, allCategories).ObjectGenerating.TryAdd(key, idClass))
                {
                    var obj = element.Key;
                    var beforeState = obj.activeInHierarchy;
                    obj.SetActive(false);
                
                    var instantiate = Object.Instantiate(obj, new Vector3(0, 0, 0), obj.transform.rotation, 
                        ObjectManager.CachedGameObjects.transform);
                
                    foreach (var componentsInChild in instantiate.GetComponentsInChildren<MeshFilter>(true))
                    {
                        if (componentsInChild.sharedMesh)
                        {
                            string meshName = componentsInChild.sharedMesh.name;
                            if (componentsInChild.sharedMesh.name.Contains("Combined Mesh"))
                                meshName = idClass.Name;
                            if (AssetManager.SeperatedMeshes.TryGetValue(meshName, out var mesh))
                                componentsInChild.sharedMesh = mesh;
                        }
                    }

                    instantiate.name = $"{idClass.Id} {obj.name}";

                    obj.SetActive(beforeState);
                    FindCategory(category, allCategories).ObjectGenerating[key].GameObject = instantiate;
                    aa++;
                }
                
            }

            bool InBounds (int index, string[] array) 
            {
                return (index >= 0) && (index < array.Length);
            }

            static IdCategoryData FindCategory(string categoryName, List<IdCategoryData> categories)
            {
                IdCategoryData category = categories.FirstOrDefault(x => x.CategoryName.Equals(categoryName));
                if (category != null) return category;
                var newCategory = new IdCategoryData
                {
                    ObjectGenerating = new Dictionary<string, IdClass>(),
                    CategoryName = categoryName
                };
                categories.Add(newCategory);
                return newCategory;
            }
        };
        foreach (var idCategoryData in allCategories)
            idCategoryData.Objects = idCategoryData.ObjectGenerating.Values.ToList();

        File.WriteAllText(SaveManager.BuildObjectsPath, JsonSerializer.Serialize(allCategories, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }
}