using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using MelonLoader;
using SRLE.Components;
using SRLE.Patches;
using UnityEngine;

namespace SRLE;

public static class SRLEManager
{
    public static GameObject AllZonesObj;
    public static GameObject World;

    public static List<BuildObjects.Category> Categories;
    public static readonly Dictionary<string, List<BuildObjects.IdClass>> SceneIdClassMapping = new Dictionary<string, List<BuildObjects.IdClass>>();
    public static readonly Dictionary<BuildObjects.IdClass, GameObject> GameObjectIdClassMapping = new Dictionary<BuildObjects.IdClass, GameObject>();

    public static GameObject SRLEGameObject;

    public static void LoadBuildObjects()
    {
        Categories = File.ReadAllText(@"D:\SteamLibrary\steamapps\common\Slime Rancher 2\SRLE\buildobjects.json").LoadFromJSON<List<BuildObjects.Category>>();
        foreach (var idClass in Categories.SelectMany(category => category.Objects))
        {
            if (!SceneIdClassMapping.TryGetValue(idClass.Scene, out var list))
            {
                list = new List<BuildObjects.IdClass>();
                SceneIdClassMapping.Add(idClass.Scene, list);
            }
            list.Add(idClass); 
        }

        AllZonesObj = new GameObject(nameof(AllZonesObj));
        AllZonesObj.hideFlags |= HideFlags.HideAndDontSave;
        AllZonesObj.SetActive(false);
        Object.DontDestroyOnLoad(AllZonesObj);



    }

    public static void ClearSRLEData()
    {
        SRLEMod.IsLoaded = false;
        
        Patch_Debug.isCreating = true;
        
        GameObjectIdClassMapping.Clear();
        Object.DestroyImmediate(AllZonesObj);
        
        AllZonesObj = new GameObject(nameof(AllZonesObj));
        AllZonesObj.hideFlags |= HideFlags.HideAndDontSave;
        AllZonesObj.SetActive(true);
        Object.DontDestroyOnLoad(AllZonesObj);
        
        World = new GameObject("World");
        World.hideFlags |= HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(World);

        
        
    }

    public static (GameObject, BuildObjects.IdClass)? GetObjectFromId(uint id)
    {
        var buildObjectId = GameObjectIdClassMapping.FirstOrDefault(x => x.Key.Id == id);
        if (buildObjectId.Key == null)
        {
            MelonLogger.Warning($"Failed to load object ID {id}");
            return null;
        }

        return (buildObjectId.Value, buildObjectId.Key);

    }

    public static void SpawnObjectFromId(uint id)
    {
        var firstOrDefault = GetObjectFromId(id);
        if (firstOrDefault != null)
        {
            var buildObjectId = Object.Instantiate(firstOrDefault.Value.Item1);
            var transformPosition = SRSingleton<SceneContext>.Instance.Player.transform.position;
            buildObjectId.transform.position = transformPosition;
            buildObjectId.SetActive(true);
            if (!SRLESaveSystem.CurrentLevel.buildObjects.TryGetValue(id, out var list))
            {
                list = new List<SRLESaveSystem.BuildObject>();
                SRLESaveSystem.CurrentLevel.buildObjects.Add(id, list);
            }
            list.Add(new SRLESaveSystem.BuildObject()
            {
                pos = buildObjectId.transform.position.ToVector3Save(),
                rot = buildObjectId.transform.eulerAngles.ToVector3Save(),
                properties = new Dictionary<string, string>(),
            });
        }
    }

    public static void RenderImages()
    {
        foreach (var VARIABLE in GameObjectIdClassMapping)
        {
            var generateModelPreview = RuntimePreviewGenerator.GenerateModelPreview(VARIABLE.Value.transform, 128, 128);
            var il2CppStructArray = ImageConversion.EncodeToPNG(generateModelPreview);
            if (il2CppStructArray is not null)
                File.WriteAllBytes($@"D:\SteamLibrary\steamapps\common\Slime Rancher 2\SRLE\Textures\{VARIABLE.Value.name}.png",il2CppStructArray);
        }
    }
    
}