using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using SRLE.Models;
using SRML.Console;
using UnityEngine;

namespace SRLE.Commands
{
    public class ConvertBetterBuildCommand : ConsoleCommand
    {
        public override bool Execute(string[] args)
        { 
            var instanceType = AccessTools.TypeByName("PmkWqSqDqhyzqncfhFgkeiIeqAFfA");
            var instance = instanceType?.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>());
            
            if (instance == null) return false;
            instanceType.GetMethod("Load", new[] { typeof(Stream) })?.Invoke(instance, new object[]
            {
                new FileStream(Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "BetterBuild", "Burning Valley.world"), FileMode.Open)
            });
            
            var nameField = instanceType.GetField("NxbuhdGOoXjvYKGyvWOfJIkcIILn");
            var dataField = instanceType.GetField("PwuTlNLrWfEhoBKNfoDdbApcvSLW");
            
            string name = (string) nameField?.GetValue(instance);
            var dataMap = dataField?.GetValue(instance) as IDictionary;
            
            var objectRegistry = AccessTools.TypeByName("TMaFbmDBkLulRlsTCbClfQhfWwmCA")
                ?.GetProperty("oMvEVShoQppTDOlWISWTPYakJbFL")?.GetValue(null) as IDictionary;
            
            var levelData = new LevelData()
            {
                LevelName = name,
                BuildObjects = new Dictionary<uint, List<BuildObjectData>>()
            };
            // if (dataMap == null || objectRegistry == null) return false;
            foreach (DictionaryEntry entry in dataMap)
            {
                var objectKey = entry.Key;
                var objectInstances = entry.Value as IList;
                
                if (!objectRegistry.Contains(objectKey) || objectInstances == null) continue;
                var registryItem = objectRegistry[objectKey];
                var registryType = registryItem.GetType();
            
                int renderId = (int) registryType.GetField("RenderID")?.GetValue(registryItem);
                string path = (string) registryType.GetField("Path")?.GetValue(registryItem);
                string bbName = (string) registryType.GetField("Name")?.GetValue(registryItem);
                uint id = (uint) registryType.GetField("ID")?.GetValue(registryItem);
            
                bool objectByHashCode = ObjectManager.TryGetObject(renderId, bbName, path, out var objectClass);
                if (!objectByHashCode)
                {
                    continue;
                }
                foreach (var instanceObj in objectInstances)
                {
                    var objType = instanceObj.GetType();
                    Vector3 position = (Vector3)objType.GetField("vnFsBQTGupjuJrOIzDrdnTGobnzCA")?.GetValue(instanceObj)?.GetType().GetField("value")?.GetValue(
                        objType.GetField("vnFsBQTGupjuJrOIzDrdnTGobnzCA")?.GetValue(instanceObj))!;
                    Vector3 rotation = (Vector3)objType.GetField("JGTZUTDwWNbSmlKDfhazTgObuUFf")?.GetValue(instanceObj)?.GetType().GetField("value")?.GetValue(
                        objType.GetField("JGTZUTDwWNbSmlKDfhazTgObuUFf")?.GetValue(instanceObj))!;
                    Vector3 scale = (Vector3)objType.GetField("oLhvsYqBzOpNVnXFhvqYIEWDabkJA")?.GetValue(instanceObj)?.GetType().GetField("value")?.GetValue(
                        objType.GetField("oLhvsYqBzOpNVnXFhvqYIEWDabkJA")?.GetValue(instanceObj))!;
            
                    if (!levelData.BuildObjects.TryGetValue(objectClass.Id, out var value))
                        levelData.BuildObjects[objectClass.Id] = value = new List<BuildObjectData>();
                    value.Add(new BuildObjectData()
                    {
                        Pos = position,
                        Rot = rotation,
                        Scale = scale,
                        Properties = new Dictionary<string, string>()
                    });
                }
                
               
            }
            
            File.WriteAllText(Path.Combine(SaveManager.LevelsPath, name + ".srle"), Newtonsoft.Json.JsonConvert.SerializeObject(levelData));
            return true;
        }

        public override string ID => "convert_better_build";
        public override string Usage => "convert_better_build [OPTIONS]";
        public override string Description => "convert_better_build";
    
    }
}