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

            using (var stream = new FileStream(Path.Combine(SaveManager.DataPath, "BB", "Worldexpansion_1.3.0_Release.world"), FileMode.Open))
                instanceType.GetMethod("Load", new[] { typeof(Stream) })?.Invoke(instance, new object[] { stream });

            string name = (string)instanceType.GetField("NxbuhdGOoXjvYKGyvWOfJIkcIILn")?.GetValue(instance);
            var dataMap = instanceType.GetField("PwuTlNLrWfEhoBKNfoDdbApcvSLW")?.GetValue(instance) as IDictionary;

            var objectRegistry = AccessTools.TypeByName("TMaFbmDBkLulRlsTCbClfQhfWwmCA")
                ?.GetProperty("oMvEVShoQppTDOlWISWTPYakJbFL")?.GetValue(null) as IDictionary;

            if (dataMap == null || objectRegistry == null) return false;

            var levelData = new LevelData
            {
                LevelName = name,
                BuildObjects = new Dictionary<uint, List<BuildObjectData>>()
            };

            foreach (DictionaryEntry entry in dataMap)
            {
                var objectKey = entry.Key;
                var objectInstances = entry.Value as IList;
                if (!objectRegistry.Contains(objectKey) || objectInstances == null || objectInstances.Count == 0) continue;

                var registryItem = objectRegistry[objectKey];
                var registryType = registryItem.GetType();
                int renderId = (int)registryType.GetField("RenderID").GetValue(registryItem);
                string path = (string)registryType.GetField("Path").GetValue(registryItem);
                string bbName = (string)registryType.GetField("Name").GetValue(registryItem);

                if (!ObjectManager.TryGetObject(renderId, bbName, path, out var objectClass)) continue;

                // Cache FieldInfo once per object type — same for every instance in this list
                var objType        = objectInstances[0].GetType();
                var posField       = objType.GetField("vnFsBQTGupjuJrOIzDrdnTGobnzCA");
                var rotField       = objType.GetField("JGTZUTDwWNbSmlKDfhazTgObuUFf");
                var scaleField     = objType.GetField("oLhvsYqBzOpNVnXFhvqYIEWDabkJA");
                var propsField     = objType.GetField("nWpPVSldMKqITEYvahfGlTmpFdlH");
                var wrapperValueField = posField?.GetValue(objectInstances[0])?.GetType().GetField("value");

                if (posField == null || rotField == null || scaleField == null || wrapperValueField == null) continue;

                // Cache the property-value wrapper FieldInfo from the first available property
                System.Reflection.FieldInfo propValueField = null;
                if (propsField?.GetValue(objectInstances[0]) is IDictionary sampleProps)
                    foreach (DictionaryEntry p in sampleProps)
                    {
                        propValueField = p.Value?.GetType().GetField("IywkVfFOJnkxgsjKznhRmgZTQrlm");
                        break;
                    }

                if (!levelData.BuildObjects.TryGetValue(objectClass.Id, out var list))
                    levelData.BuildObjects[objectClass.Id] = list = new List<BuildObjectData>();

                foreach (var instanceObj in objectInstances)
                {
                    if (instanceObj == null) continue;
                    var position = (Vector3)wrapperValueField.GetValue(posField.GetValue(instanceObj));
                    var rotation = (Vector3)wrapperValueField.GetValue(rotField.GetValue(instanceObj));
                    var scale    = (Vector3)wrapperValueField.GetValue(scaleField.GetValue(instanceObj));

                    var ourProperties = new Dictionary<string, string>();
                    var props = propsField?.GetValue(instanceObj) as IDictionary;
                    if (props != null && propValueField != null)
                        foreach (DictionaryEntry prop in props)
                        {
                            ourProperties[(string)prop.Key] = (string)propValueField.GetValue(prop.Value);
                            EntryPoint.ConsoleInstance.Log((string)prop.Key +" " +  (string)propValueField.GetValue(prop.Value));

                        }

                    list.Add(new BuildObjectData
                    {
                        Pos = position,
                        Rot = rotation,
                        Scale = scale,
                        Properties = ourProperties
                    });
                }
            }

            File.WriteAllText(Path.Combine(SaveManager.LevelsPath, "Worldexpansion_1.3.0_Release" + ".srle"), Newtonsoft.Json.JsonConvert.SerializeObject(levelData));
            return true;
        }

        public override string ID => "convert_better_build";
        public override string Usage => "convert_better_build [OPTIONS]";
        public override string Description => "convert_better_build";
    
    }
}