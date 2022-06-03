using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SRML.Console;
using UnityEngine;
using static SRLE.EntryPoint;

namespace SRLE
{
    public class SRLECommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        {
            List<string> sectors =
                "Main Nav|Crystals|Cliffs|Mountains|Solid Filler|Rocks|Ranch Features|Doors|Trees|Ceiling|Flora|Grass|Deco|Constructs|Resources|Slimes|FX|Lights|Water|Colliders|Loot|Audio|Build Sites|Interactives|Upgrades|Cave Roof|Race Waypoints|ValleyAmmoSwapTrigger|Giant Trees|Main Nav Internal|Solid FIller|Constructions|".Split('|').ToList();
            List<int> hashcodes = new List<int>();
            List<GameObject> listOfCategory = new List<GameObject>();

                foreach (var cellDirector in Resources.FindObjectsOfTypeAll<CellDirector>())
                {
                    var sector = cellDirector.transform.Find("Sector");
                    if (sector is null) continue;
                    foreach (Transform elements in sector)
                    {


                        if (sectors.Exists(s => s == elements.gameObject.name))
                        {
                            
                            if (sector.transform.parent.parent is null) continue;
                            foreach (Transform element in elements)
                            {


                                bool ignore = true;
                  
                                int num2 = -1;
                                if (element.name is "overgrowth_lv0" or "overgrowth_lv1" or "grotto_lv1" or "docks_lv1" or "docks_lv0")
                                {
                                    ignore = false;
                                    
                                    
                                    foreach (Transform miniElement in element)
                                    {
                                        string text = "";
                                        MeshFilter[] meshFilters = miniElement.GetComponentsInChildren<MeshFilter>();
                                        Renderer[] renderers = miniElement.GetComponentsInChildren<Renderer>();
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
                                        
                                        if (num2 != -1)
                                        {
                                            listOfCategory.Add(miniElement.gameObject);   
                                        }
                                        
                                        

                  
                                    }


                                }
                                
                                if (element.childCount <= 0 && ignore)
                                {

                                    MeshFilter meshFilter = element.GetComponent<MeshFilter>();
                                    Renderer renderer = element.GetComponent<Renderer>();
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
                                    MeshFilter[] meshFilters = element.GetComponentsInChildren<MeshFilter>();
                                    Renderer[] renderers = element.GetComponentsInChildren<Renderer>();
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
                                    listOfCategory.Add(element.gameObject);   
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
                

                
                List<Category> categories = new List<Category>();


                static Category FindCategory(string CategoryName, List<Category> categories)
                {
                    Category category = null;
                    foreach (var VARIABLE in categories)
                    {
                        if (VARIABLE.CategoryName == CategoryName)
                            category = VARIABLE;
                    }

                    if (category is null)
                    {
                        var item = new Category {Objects = new List<IdClass>(), CategoryName = CategoryName};
                        categories.Add(item);
                        category = item;
                    }

                    return category;




                }

                foreach (var element in listOfCategory)
                {
                    var path = element.GetFullName();
                    var strings = path.Split('/', '/');
                    var category = InBounds(3, strings) ? strings[3] : "None";
                    var idClass = new IdClass {Id = aa, Name = element.gameObject.name, Path = path};
                    FindCategory(category, categories).Objects.Add(idClass);
                    aa++;
                }

                File.WriteAllText(@"E:\SteamLibrary\steamapps\common\Slime Rancher\SRLE\BuildObjects\buildobjects.txt", JsonConvert.SerializeObject(categories, Formatting.Indented));
                return true;
        }

        public override string ID => "build_buildobjects";
        public override string Usage => ID;
        public override string Description => ID;
    }
}