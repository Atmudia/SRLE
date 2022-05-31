using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SRML;
using SRML.SR;
using SRML.SR.UI.Utils;
using SRML.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Console = SRML.Console.Console;

namespace SRLE
{
    public class EntryPoint : ModEntryPoint
    {
        public static Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
        public static Dictionary<string, int> dictionary2 = new Dictionary<string, int>();

        public override void PreLoad()
        {

            SRCallbacks.OnMainMenuLoaded += menu =>
            {

                var c = PrefabUtils.CopyPrefab(SRObjects.Get<ExpoGameSelectUI>().gameObject);
                var addMainMenuButton = MainMenuUtils.AddMainMenuButton(menu, "SRLE", () =>
                {
                    /*Destroyer.Destroy(Object.FindObjectOfType<MainMenuUI>().gameObject, "SRLE.Button");
                    menu.InstantiateAndWaitForDestroy(c);
                    */
                });
                addMainMenuButton.GetComponent<Button>().onClick =
                    menu.transform.Find("ExpoModePanel/PlayButton").GetComponent<Button>().onClick;
                addMainMenuButton.name = "SRLEButton";
                addMainMenuButton.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
                addMainMenuButton.transform.SetSiblingIndex(4);


            };
            SRCallbacks.PreSaveGameLoad += _ =>
            {



                List<string> sectors =
                    "Ranch Features|Constructs|Build Sites|Main Nav|Constructions|Solid Filler|Rocks|Flora|Cliffs|Mountains|Grass|Deco|Resources|FX|Lights|Water|Colliders|Loot|Audio|Interactives|Upgrades|Cave Roof|Giant Trees"
                        .Split('|').ToList();


                static void Recursion(GameObject gameObject, GameObject child, string name)
                {
                    if (gameObject is null) gameObject = dictionary[name];
                    if (gameObject.TryGetComponent(out MeshRenderer meshRenderer))
                        if (child.TryGetComponent(out MeshRenderer meshRenderer2))
                        {
                            if (meshRenderer.sharedMaterial != meshRenderer2.sharedMaterial)
                            {
                                int increasement = dictionary2[name] + 1;
                                dictionary2[name] = increasement;
                                dictionary.Add(child.name + " (" + increasement + ")", child.gameObject);
                            }
                        }
                }
                

                //var makeSnapshotCamera = SnapshotCamera.MakeSnapshotCamera();
                foreach (var cellDirector in Resources.FindObjectsOfTypeAll<CellDirector>())
                {
                    var transform = cellDirector.transform.Find("Sector");
                    if (transform is null) continue;
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        var sectorGameObject = transform.GetChild(i).gameObject;
                        if (sectors.Exists(s => s == sectorGameObject.name))
                        {
                            foreach (Transform child in sectorGameObject.transform)
                            {
                                string name = child.gameObject.name;
                                if (name.Contains("(") )
                                {
                                    var oldValue = Regex.Match(name, @"\(\d+\)").Value;
                                    if (!string.IsNullOrEmpty(oldValue))
                                        name = name.Replace(oldValue, string.Empty);
                                    
                                }
                                if (dictionary.TryAdd(name, child.gameObject))
                                    dictionary2.Add(name, 0);
                                else
                                {
                                    var gameObject = dictionary[name];
                                    if (gameObject.name == name) continue;
                                    if (gameObject.TryGetComponent(out MeshRenderer meshRenderer) && child.TryGetComponent(out MeshRenderer meshRenderer2))
                                        if (meshRenderer.sharedMaterial != meshRenderer2.sharedMaterial)
                                        {
                                            dictionary2[name]++;
                                            dictionary.Add(name + " " + $"({dictionary2[name]})", child.gameObject);
                                            continue;
                                        }
                                    if (gameObject.TryGetComponent(out MeshFilter meshFilter) && child.TryGetComponent(out MeshFilter meshFilter2))
                                        if (meshFilter.sharedMesh != meshFilter2.sharedMesh)
                                        {
                                            dictionary2[name]++;
                                            dictionary.Add(name + " " + $"({dictionary2[name]})", child.gameObject);
                                        }

                                }

                            }
                        }
                    }
                    
                    
                }
                uint id = 1000000;
            };

        }
    }
}