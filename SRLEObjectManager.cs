using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppInterop.Runtime;
using MelonLoader;
using SRLE.Components;
using UnityEngine;

namespace SRLE;

public static class SRLEObjectManager
{
    public static Dictionary<string, List<BuildObjects.IdClass>> SortedCategories;
    public static BuildObjects.IdClass[] OnlyIdClasses;
    public static GameObject CachedGameObjects;
    private static AssetBundle srleAssetBundle;
    public static Shader[] Shaders;

    public static void LoadBuildObjects()
    {
        SortedCategories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BuildObjects.Category>>(File.ReadAllText(SRLESaveManager.BuildObjectsPath)).ToDictionary(x => x.CategoryName, x => x.Objects);
        OnlyIdClasses = SortedCategories.Values.ToArray().SelectMany(x => x).ToArray();
        srleAssetBundle = UnityEngine.AssetBundle.LoadFromFile(@"C:\Users\komik\SRLEGizmo\Assets\AssetBundles\srle");
        Shaders = srleAssetBundle.LoadAllAssets(Il2CppType.Of<Shader>()).Select(delegate(Object o)
        {
            var shader = o.Cast<Shader>();
            shader.hideFlags |= HideFlags.HideAndDontSave;
            return shader;
        }).ToArray();
    }

    public static void SpawnObject(uint id, BuildObject buildObject = null, bool async = true)
    {
        var findObject = FindObject(id);
        if (findObject == null)
            return;
        if (async)
        {
            MelonCoroutines.Start(SpawnObjectWithAsync(findObject,  buildObject));
            return;
        }

        var gameObject = UnityEngine.Object.Instantiate(findObject.GameObject, SRLEManager.World.transform, true);
        
        gameObject.transform.position = buildObject == null ? SRLECamera.Instance.transform.position : buildObject.Pos.ToVector3();
        gameObject.transform.rotation = buildObject == null ? SRLECamera.Instance.transform.rotation : Quaternion.Euler(buildObject.Rot.ToVector3());
        gameObject.transform.localScale = buildObject == null ? Vector3.zero : buildObject.Scale.ToVector3();
        gameObject.SetActive(true);
        gameObject.AddComponent<BuildObjectId>().IdClass = findObject;
        if (buildObject == null)
        {
            if (!SRLESaveManager.CurrentLevel.BuildObjects.TryGetValue(id, out var value))
            {
                var buildObjects = new List<BuildObject>();
                value = buildObjects;
                SRLESaveManager.CurrentLevel.BuildObjects.Add(id, value);
            }
            value.Add(new BuildObject()
            {
                Pos = gameObject.transform.position.ToVector3Save(),
                Rot = gameObject.transform.localEulerAngles.ToVector3Save(),
                Scale = gameObject.transform.localScale.ToVector3Save(),
                Properties = new Dictionary<string, string>()
            });
            MelonLogger.Msg($"Spawned with: {gameObject.name} ");

        }
    }
    public static IEnumerator SpawnObjectWithAsync(BuildObjects.IdClass id, BuildObject buildObject)
    {
        var gameObject = UnityEngine.Object.Instantiate(id.GameObject, SRLEManager.World.transform, true);
        gameObject.transform.position = buildObject == null ? SRLECamera.Instance.transform.position : buildObject.Pos.ToVector3();
        gameObject.transform.rotation = buildObject == null ? SRLECamera.Instance.transform.rotation : Quaternion.Euler(buildObject.Rot.ToVector3());
        gameObject.transform.localScale = buildObject == null ? id.GameObject.transform.localScale : buildObject.Scale.ToVector3();
        gameObject.SetActive(true);
        gameObject.AddComponent<BuildObjectId>().IdClass = id;
        if (buildObject == null)
        {
            if (!SRLESaveManager.CurrentLevel.BuildObjects.TryGetValue(id.Id, out var value))
            {
                var buildObjects = new List<BuildObject>();
                value = buildObjects;
                SRLESaveManager.CurrentLevel.BuildObjects.Add(id.Id, value);
            }
            value.Add(new BuildObject()
            {
                Pos = gameObject.transform.position.ToVector3Save(),
                Rot = gameObject.transform.localEulerAngles.ToVector3Save(),
                Scale = gameObject.transform.localScale.ToVector3Save(),
                Properties = new Dictionary<string, string>()
            });
        }
        MelonLogger.Msg($"Spawned with async method: {gameObject.name} ");
        yield return null;

    }

    
    public static BuildObjects.IdClass FindObject(uint id)
    {
        var idClass = OnlyIdClasses.FirstOrDefault(x => x.Id == id);
        if (idClass == null)
        {
            MelonLogger.Warning($"Can't find object with id: {id}");
            return null;
        }

        return idClass;
    }

}