using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRLE;

public static class SRLEConverter
{
    private static List<string> included = "Main Nav|Cliffs|Mountains|Solid Filler|Rocks|Flora|Grass|Deco|Constructs|Resources|Slimes|FX|Lights|Water|Colliders|Loot|Audio|Build Sites|Roots|Upgrades|Ranch Features|Drone Network".Split('|').ToList();
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
                    if (cellDirector.transform.parent is not null)
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
                                    num2 = (meshFilter.sharedMesh.name + renderer.material.name).GetHashCode();
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
                                    
            List<BuildObjects.Category> categories = new List<BuildObjects.Category>();
            static BuildObjects.Category FindCategory(string CategoryName, List<BuildObjects.Category> categories)
            {
                    
                BuildObjects.Category category = null;
                foreach (var VARIABLE in categories)
                {
                    if (VARIABLE.CategoryName == CategoryName)
                        category = VARIABLE;
                }

                if (category is null)
                {
                    var item = new BuildObjects.Category {Objects = new List<BuildObjects.IdClass>(), CategoryName = CategoryName};
                    categories.Add(item);
                    category = item;
                }

                return category;




            }

            foreach (var element in listOfCategory)
            {
                var path = element.Key.GetFullName();
                var strings = path.Split('/', '/');
                var category = InBounds(3, strings) ? strings[3] : "None";
                var idClass = new BuildObjects.IdClass {Id = aa.ToString(), Name = element.Key.name, Path = path, Scene = element.Key.scene.name, HashCode = element.Value};
                FindCategory(category, categories).Objects.Add(idClass);
                aa++;
            }
            File.WriteAllText(@"E:\SteamLibrary\steamapps\common\Slime Rancher 2\SRLE\buildobjects.json", JsonConvert.SerializeObject(categories, Formatting.Indented));
        };

    }
}