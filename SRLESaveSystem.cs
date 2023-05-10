using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Il2CppMonomiPark.SlimeRancher.Persist;
using MelonLoader;
using SRLE.Patches;
using UnityEngine;

namespace SRLE;

public static class SRLESaveSystem
{
    public static WorldV01 CurrentLevel;

    public class Vector3Save
    {
        public float x, y, z;
    }
    public class WorldV01
    {
        public string worldName;
        public Dictionary<string, string> dependencies;
        public Dictionary<uint, List<BuildObject>> buildObjects;


    }
    public class BuildObject
    {
        public Vector3Save pos;
        public Vector3Save rot;
        public Dictionary<string, string> properties;
    }



    public static void LoadLevel(string currentPath)
    {
        if (!File.Exists(currentPath))
        {
            throw new FileNotFoundException("Level was not found, will not be loaded.");
        }

        CurrentLevel = File.ReadAllText(currentPath).LoadFromJSON<WorldV01>();
        
    }
    public static void SaveLevel(string currentPath)
    {
        CurrentLevel.SaveToJSON(currentPath);
    }
    public static void CreateLevel(string worldName)
    {
        CurrentLevel = new WorldV01()
        {
            worldName = worldName,
            buildObjects = new Dictionary<uint, List<BuildObject>>(),
            
        };
    }

    private static IEnumerator InstantiateObject(GameObject objectFromId, BuildObject data, BuildObjects.IdClass idClass)
    {
        var gameObject = UnityEngine.Object.Instantiate(objectFromId.gameObject, SRLEManager.World.transform, true);
        gameObject.transform.position = data.pos.ToVector3Save();
        gameObject.transform.rotation = Quaternion.Euler(data.rot.ToVector3Save());
        gameObject.active = true;
        gameObject.AddComponent<BuildObjectId>().IdClass = idClass;
        yield return null;
    }

    public static void LoadObjectsFromLevel()
    {
        
        foreach (var id in CurrentLevel.buildObjects.Keys)
        {
            var objectFromId = SRLEManager.GetObjectFromId(id);
            if (objectFromId == null) continue;
            foreach (var data in CurrentLevel.buildObjects[id])
            {
                MelonCoroutines.Start(InstantiateObject(objectFromId.Value.Item1, data, objectFromId.Value.Item2));

            }
        }
    }
}