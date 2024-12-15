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
    public static List<IdCategoryData> CategoryDatas;
    
    public static bool IsConverting = false;

    private static List<string> included = new()
    {
        "Main Nav", "Cliffs", "Mountains", "Solid Filler", "Rocks", "Flora", "Grass",
        "Deco", "Constructs", "Resources", "Slimes", "FX", "Lights", "Water",
        "Colliders", "Loot", "Audio", "Build Sites", "Roots", "Upgrades",
        "Ranch Features", "Drone Network"
    };
    public static string GetFullName(this GameObject obj)
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
    public static void ConvertToBuildObjects()
    {
        List<int> hashcodes = new List<int>();
        Dictionary<GameObject, int> listOfCategory = new Dictionary<GameObject, int>();
        foreach (var scene in GetAllScenes().Where(x => !x.name.EndsWith("Core")))
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var cellDirector in rootGameObject.GetComponentsInChildren<CellDirector>())
                {
                    if (cellDirector.transform.parent != null)
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
                                if (renderer != null && meshFilter != null)
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

                                    if (meshFilter.sharedMesh is not null)
                                    {
                                        text += meshFilter.sharedMesh.name;
                                    }
                                }
                                foreach (var renderer in renderers)
                                {
                                    if (renderer.material is not null)
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
            uint aa = 1;

            bool InBounds (int index, string[] array) 
            {
                return (index >= 0) && (index < array.Length);
            }

            List<IdCategoryData> categories = new List<IdCategoryData>();
            // static IdCategoryData FindCategory(string CategoryName, List<IdCategoryData> categories)
            // {
            //     IdCategoryData category = categories.FirstOrDefault(x => x.CategoryName.Equals(CategoryName));
            //     if (category != null)
            //     {
            //         var item = new IdCategoryData {Objects = new List<IdClass>(), CategoryName = CategoryName};
            //         categories.Add(item);
            //         category = item;
            //     }
            //     // MelonLogger.Msg(category.Objects == null);
            //     return category;
            // }
            static IdCategoryData FindCategory(string categoryName, List<IdCategoryData> categories)
            {
                IdCategoryData category = categories.FirstOrDefault(x => x.CategoryName.Equals(categoryName));
                if (category != null) return category;
                var newCategory = new IdCategoryData
                {
                    Objects = [],
                    CategoryName = categoryName
                };
                categories.Add(newCategory);
                return newCategory;
            }


            foreach (var element in listOfCategory)
            {
                var path = element.Key.GetFullName();
                var strings = path.Split('/', '/');
                var category = InBounds(3, strings) ? strings[3] : "None";
                var idClass = new IdClass {Id = aa, Name = element.Key.name, Path = path, Scene = element.Key.scene.name, HashCode = element.Value};
                FindCategory(category, categories).Objects.Add(idClass);
                // MelonLogger.Msg(category == null);

                aa++;
            }
            File.WriteAllText(@"BuildObjects.json", JsonSerializer.Serialize(categories, new JsonSerializerOptions()
            {
                WriteIndented = true
            }));
            CategoryDatas = categories;
        };

    }
}