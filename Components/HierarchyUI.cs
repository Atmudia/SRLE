using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher.UI;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using IntPtr = System.IntPtr;

namespace SRLE.Components;

public class HierarchyUI : BaseUI
{
    public static HierarchyUI Instance;
    private GameObject Hierarchy;
    private ScrollRect CategoryScroll;
    private ScrollRect ObjectsScroll;
    private TMP_InputField SearchInput;
    private Dictionary<uint,Texture2D> BuildObjectsPreview;


    public HierarchyUI(IntPtr value) : base(value)
    {
    }

    public override void Awake()
    {
        Instance = this;
        if (!Directory.Exists(Path.Combine(SRLESaveManager.DataPath, "Textures")))
            Directory.CreateDirectory(Path.Combine(SRLESaveManager.DataPath, "Textures"));
        Hierarchy = transform.Find("Hierarchy").gameObject;
        CategoryScroll = transform.Find("Hierarchy/CategoryScroll").GetComponent<ScrollRect>();
        ObjectsScroll = transform.Find("Hierarchy/ObjectsScroll").GetComponent<ScrollRect>();
        SearchInput = transform.Find("Hierarchy/SearchInput").GetComponent<TMP_InputField>();

        BuildObjectsPreview = new Dictionary<uint, Texture2D>();
        for (int i = 0; i < CategoryScroll.content.childCount; i++)
        {
            Transform child = CategoryScroll.content.GetChild(i);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < ObjectsScroll.content.childCount; i++)
        {
            Transform child = ObjectsScroll.content.GetChild(i);
            Destroy(child.gameObject);
        }
        SearchInput.onValueChanged.AddListener(new System.Action<string>(Search));
        
        GameObject favoritesButton = Instantiate(SRLEAssetManager.CategoryButtonPrefab, CategoryScroll.content, false);

        favoritesButton.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() =>
        {
            SelectCategory("Favorites");
        }));
        favoritesButton.GetComponentInChildren<Text>().text = "Favorites";
        foreach (string categoryName in SRLEObjectManager.BuildCategories.Keys)
        {
            GameObject categoryObj = Instantiate(SRLEAssetManager.CategoryButtonPrefab, CategoryScroll.content, false);

            categoryObj.GetComponentInChildren<Button>().onClick.AddListener(new System.Action(() => SelectCategory(categoryName)));
            categoryObj.GetComponentInChildren<Text>().text = categoryName;
        }
    }
    public static Bounds CalculateObjectBounds(GameObject go)
    {
        Bounds combinedBounds = new Bounds(Vector3.zero, Vector3.zero);

        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            if (rend.enabled)
            {
                // Transformuj granice renderera do przestrzeni światowej
                Bounds rendererBounds = rend.bounds;
                rendererBounds.center = rend.transform.TransformPoint(rendererBounds.center);

                // Rozszerz łączne granice, aby zawierały granice renderera
                combinedBounds.Encapsulate(rendererBounds);
            }
        }

        return combinedBounds;
    }
    
    private void Search(string term)
    {
        for (int i = 0; i < ObjectsScroll.content.childCount; i++)
        {
            Transform child = ObjectsScroll.content.GetChild(i);
            Destroy(child.gameObject);
        }

        if (term.Length < 2) return;

        foreach (var buildObject in SRLEObjectManager.BuildObjectsData.Values.ToList())
        {
            if(buildObject.Name.ToLower().Contains(term.ToLower()))
            {
                var objectID = buildObject.Id;
                var objectName = buildObject.Name;

                GameObject buildObj = Instantiate(SRLEAssetManager.ObjectButtonPrefab, ObjectsScroll.content, false);
                buildObj.GetComponentInChildren<Button>().onClick.AddListener(new Action(() => SpawnObject(objectID)));
                buildObj.GetComponentInChildren<Text>().text = objectName;
                var favorite = buildObj.transform.Find("Favorite");
                favorite.GetComponent<Image>().color = SRLEObjectManager.BuildCategories["Favorites"].Contains(objectID) ? Color.yellow : Color.gray;
                favorite.GetComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    if (!SRLEObjectManager.BuildCategories["Favorites"].Contains(objectID))
                    {
                        SRLEObjectManager.BuildCategories["Favorites"].Add(objectID);
                        favorite.GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        SRLEObjectManager.BuildCategories["Favorites"].Remove(objectID);
                        favorite.GetComponent<Image>().color = Color.gray;
                    }
                    File.WriteAllText(Path.Combine(SRLESaveManager.DataPath, "favorites.txt"), JsonConvert.SerializeObject(SRLEObjectManager.BuildCategories["Favorites"]));
  
                }));

                if (!BuildObjectsPreview.ContainsKey(objectID))
                {
                    if (File.Exists(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg")))
                    {
                        byte[] bytes = File.ReadAllBytes(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg"));

                        var result = new Texture2D(64, 64, TextureFormat.RGB24, false);
                        result.LoadImage(bytes, false);
                        result.Apply(false, false);

                        buildObj.GetComponentInChildren<RawImage>().texture = result;

                        BuildObjectsPreview.Add(objectID, result);
                    }
                    else
                    {
                        SRLEObjectManager.RequestObject(objectID, (previewObj) =>
                        {
                            if (previewObj == null) return;
                            if (buildObj == null) return;

                            var bounds = CalculateObjectBounds(previewObj);
                            Texture2D texture = previewObj.RenderImage(new RuntimePreviewGeneratorAidanNotworking.RenderConfig(64, 64, Camera.main.transform.rotation)
                            {
                                centerOverride = bounds.center,
                                renderHeightOverride = bounds.max.y - bounds.min.y
                            }, out var ex);
                            MelonLogger.Error(ex);
                            byte[] bytes = texture.EncodeToPNG();
                            File.WriteAllBytes(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg"), bytes);

                            buildObj.GetComponentInChildren<RawImage>().texture = texture;

                            BuildObjectsPreview.Add(objectID, texture);
                        });
                    }
                }
                else
                {
                    buildObj.GetComponentInChildren<RawImage>().texture = BuildObjectsPreview[objectID];
                }
            }
        }
    }

    private void SpawnObject(uint id)
    {
        SRLEObjectManager.SpawnObject(id);
    }
    private void SelectCategory(string categoryName)
    {
        if (SRLEObjectManager.BuildCategories.TryGetValue(categoryName, out List<uint> categoryObjects))
        {
            for (int i = 0; i < ObjectsScroll.content.childCount; i++)
            {
                Transform child = ObjectsScroll.content.GetChild(i);
                Destroy(child.gameObject);
            }

            foreach (var objectID in categoryObjects)
            {
                if (!SRLEObjectManager.BuildObjectsData.ContainsKey(objectID)) continue;

                var objectName = SRLEObjectManager.BuildObjectsData[objectID].Name;

                GameObject buildObj = Instantiate(SRLEAssetManager.ObjectButtonPrefab, ObjectsScroll.content, false);
                buildObj.GetComponentInChildren<Button>().onClick.AddListener( new Action(() => SpawnObject(objectID)));
                buildObj.GetComponentInChildren<Text>().text = objectName;
                var favorite = buildObj.transform.Find("Favorite");
                favorite.GetComponent<Image>().color = SRLEObjectManager.BuildCategories["Favorites"].Contains(objectID) ? Color.yellow : Color.gray;
                favorite.GetComponent<Button>().onClick.AddListener(new Action(() =>
                {
                    if (!SRLEObjectManager.BuildCategories["Favorites"].Contains(objectID))
                    {
                        SRLEObjectManager.BuildCategories["Favorites"].Add(objectID);
                        favorite.GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        SRLEObjectManager.BuildCategories["Favorites"].Remove(objectID);
                        favorite.GetComponent<Image>().color = Color.gray;
                    }
                    File.WriteAllText(Path.Combine(SRLESaveManager.DataPath, "favorites.txt"), JsonConvert.SerializeObject(SRLEObjectManager.BuildCategories["Favorites"]));
                    if (categoryName.Equals("Favorites"))
                    {
                        Destroy(buildObj);
                    }
                }));


                if (!BuildObjectsPreview.ContainsKey(objectID))
                {
                    if (File.Exists(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg")))
                    {
                        byte[] bytes = File.ReadAllBytes(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg"));

                        var result = new Texture2D(64, 64, TextureFormat.RGB24, false);
                        result.LoadImage(bytes, false);
                        result.Apply(false, false);

                        buildObj.GetComponentInChildren<RawImage>().texture = result;

                        BuildObjectsPreview.Add(objectID, result);
                    }
                    else
                    {
                        SRLEObjectManager.RequestObject(objectID, (previewObj) =>
                        {
                            if (previewObj == null) return;
                            if (buildObj == null) return;

                            var bounds = CalculateObjectBounds(previewObj);
                            Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(previewObj.transform);

                            byte[] bytes = texture.EncodeToPNG();
                            File.WriteAllBytes(Path.Combine(SRLESaveManager.DataPath, "Textures", objectID + ".jpg"), bytes);

                            buildObj.GetComponentInChildren<RawImage>().texture = texture;

                            BuildObjectsPreview.Add(objectID, texture);
                        });
                    }
                }
                else
                {
                    buildObj.GetComponentInChildren<RawImage>().texture = BuildObjectsPreview[objectID];
                }
            }
        }
    }

}