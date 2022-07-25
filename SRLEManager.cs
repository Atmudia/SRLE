using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Compilation;
using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.SaveSystem;
using SRML;
using SRML.Utils;
using UnityEngine;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;

namespace SRLE
{
    public static class SRLEManager
    {
        public static Dictionary<string, Dictionary<string, IdClass>> BuildObjects =
            new Dictionary<string, Dictionary<string, IdClass>>();

        public static SRLEName currentData;
        public static bool isSRLELevel;

        public static DirectoryInfo SRLE = new DirectoryInfo(Application.streamingAssetsPath.Replace("/SlimeRancher_Data/StreamingAssets", string.Empty) + "\\SRLE");

        public static DirectoryInfo Worlds = SRLE.CreateSubdirectory("Worlds");
        public static DirectoryInfo Icons = SRLE.CreateSubdirectory("Icons");

        public static GameObject DontDestroyObjects;





        internal static string GetObjectByHashCode(int hashCode, string path, string name)
        {
            foreach (var buildObjectsKey in SRLEManager.BuildObjects.Values)
            {
                foreach (var keyValue in buildObjectsKey)
                {
                    if (keyValue.Value.HashCode == hashCode)
                    {
                        return keyValue.Value.Id;
                    }
                }
            }

            return GetObjectByName(name, path, hashCode);
        }



        public static IdClass GetObjectById(string id)
        {
            Console.Log($"Trying to find object with id: {id}");
            foreach (var buildObjectsKey in SRLEManager.BuildObjects.Values)
            {
                foreach (var keyValue in buildObjectsKey)
                {
                    if (keyValue.Value.Id == id)
                    {
                        Console.Log($"Found id is: {keyValue.Value.Id} by method id");

                        return keyValue.Value;
                    }
                }
            }

            return null;
        }

        internal static string GetObjectByName(string name, string path, int hashCode)
        {
            Console.Log($"Trying other method: {name}");
            foreach (var buildObjectsKey in SRLEManager.BuildObjects.Values)
            {
                foreach (var keyValue in buildObjectsKey)
                {
                    if (keyValue.Value.Name == name)
                    {
                        Console.Log($"Found id is: {keyValue.Value.Id} by method name");

                        return keyValue.Value.Id;
                    }
                }
            }

            return GetObjectByPath(path);
        }

        internal static string GetObjectByPath(string path)
        {
            foreach (var buildObjectsKey in SRLEManager.BuildObjects.Values)
            {
                foreach (var keyValue in buildObjectsKey)
                {
                    if (keyValue.Value.Path == path)
                    {
                        Console.Log($"Found id is: {keyValue.Value.Id} by method path");
                        return keyValue.Value.Id;
                    }
                }
            }

            Console.Log($"Please contact with SRLE Team to resolve this problem " + $" #Path {path}");

            return string.Empty;
        }

        internal static void LoadObjectsFromBuildObjects()
        {


            SRLEManager.DontDestroyObjects = new GameObject("[SRLE] DontDestroyObject", typeof(ContainersOfObject));
            Object.DontDestroyOnLoad(SRLEManager.DontDestroyObjects);
            List<Category> categories;
            using (StreamReader streamReader =
                new StreamReader(
                    EntryPoint.execAssembly!.GetManifestResourceStream(typeof(EntryPoint), "buildobjects.txt")!))
            {
                categories = new JsonSerializer().Deserialize<List<Category>>(new JsonTextReader(streamReader));
            }

            foreach (var category in categories)
            {
                foreach (var idClass in category.Objects)
                {
                    if (!BuildObjects.ContainsKey(category.CategoryName))
                        BuildObjects.Add(category.CategoryName, new Dictionary<string, IdClass>
                        {
                            {idClass.Id, idClass}
                        });
                    else
                    {
                        BuildObjects[category.CategoryName].Add(idClass.Id, idClass);
                    }
                }
            }
        }

        internal static bool SaveLevel()
        {
            if (!isSRLELevel) return false;


            foreach (var objects in currentData.objects)
            {
                objects.Value.ForEach(save =>
                {

                    if (save.modid != "none")
                    {
                        currentData.isUsingModdedObjects = true;
                    }
                    else
                    {
                        currentData.isUsingModdedObjects = false;
                    }
                });


            }


            var fileInfo = new FileInfo(Worlds.FullName + "\\" + currentData.nameOfLevel + ".srle");
            var fileStream = fileInfo.Open(FileMode.OpenOrCreate);
            "Saving SRLE Level".LogWarning();
            currentData.Write(fileStream);
            "Completed SRLE Level".LogWarning();

            fileStream.Dispose();
            return true;

        }




        public static void AddModdedObject(GameObject objectToAdd)
        {
            
                var instantiateInactive = GameObjectUtils.InstantiateInactive(objectToAdd);
                instantiateInactive.name = instantiateInactive.name.Replace("(Clone)", string.Empty);
                instantiateInactive.transform.SetParent(DontDestroyObjects.transform);

                var modInfoName = SRMod.GetCurrentMod().ModInfo.Id;
                if (!BuildObjects.ContainsKey("Mods"))
                {

                    ModdedIdClass moddedIdClass = new ModdedIdClass
                        {modid = modInfoName, Id = $"mod.1", Name = objectToAdd.name, Path = objectToAdd.GetFullName()};

                    BuildObjects.Add("Mods", new Dictionary<string, IdClass>
                    {
                        {moddedIdClass.Id, moddedIdClass}
                    });
                }
                else
                {
                    var count = BuildObjects["Mods"].Count;
                    count++;

                    string idOfSRLE = $"mod.{count}";
                    ModdedIdClass moddedIdClass = new ModdedIdClass
                        {modid = modInfoName, Id = idOfSRLE, Name = objectToAdd.name, Path = objectToAdd.GetFullName()};

                    BuildObjects["Mods"].Add(moddedIdClass.Id, moddedIdClass);
                }

        }
    }
}