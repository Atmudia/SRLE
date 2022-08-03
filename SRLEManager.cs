using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

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

        public static DirectoryInfo SRLE = new DirectoryInfo(System.Environment.CurrentDirectory + "\\SRLE");

        public static DirectoryInfo Worlds = SRLE.CreateSubdirectory("Worlds");
        public static DirectoryInfo Icons = SRLE.CreateSubdirectory("Icons");

        public static GameObject DontDestroyObjects;





        internal static string GetObjectByHashCode(string name, string path, int hashCode)
        {
            foreach (var keyValue in from buildObjectsKey in SRLEManager.BuildObjects.Values from keyValue in buildObjectsKey where keyValue.Value.HashCode == hashCode select keyValue)
            {
                return keyValue.Value.Id;
            }

            return GetObjectByPath(hashCode, path, name );
        }



        public static IdClass GetObjectById(string id)
        {
            EntryPoint.SRLEConsoleInstance.Log($"Trying to find object with id: {id}");
            foreach (var keyValue in from buildObjectsKey in SRLEManager.BuildObjects.Values from keyValue in buildObjectsKey where keyValue.Value.Id == id select keyValue)
            {
                EntryPoint.SRLEConsoleInstance.Log($"Found id is: {keyValue.Value.Id} by method id");

                return keyValue.Value;
            }

            return null;
        }

        internal static string GetObjectByName(string name, string path, int hashCode)
        {

            EntryPoint.SRLEConsoleInstance.Log($"Trying other method: {name}");
            foreach (var keyValue in from buildObjectsKey in SRLEManager.BuildObjects.Values from keyValue in buildObjectsKey where keyValue.Value.Name == name select keyValue)
            {
                EntryPoint.SRLEConsoleInstance.Log($"Found id is: {keyValue.Value.Id} by method name");

                return keyValue.Value.Id;
            }
            EntryPoint.SRLEConsoleInstance.Log($"Please contact with SRLE Team to resolve this problem " + $" #Path {path}");

            return string.Empty;
        }

        internal static string GetObjectByPath(int hashCode, string path, string name)
        {
            foreach (var keyValue in from buildObjectsKey in SRLEManager.BuildObjects.Values from keyValue in buildObjectsKey where keyValue.Value.Path == path select keyValue)
            {
                EntryPoint.SRLEConsoleInstance.Log($"Found id is: {keyValue.Value.Id} by method path");
                return keyValue.Value.Id;
            }


            return GetObjectByName(name, path, hashCode);
        }

        internal static void LoadObjectsFromBuildObjects()
        {


            SRLEManager.DontDestroyObjects = new GameObject("[SRLE] DontDestroyObject", typeof(ContainersOfObject));
            Object.DontDestroyOnLoad(SRLEManager.DontDestroyObjects);
            List<Category> categories;
            using (var streamReader = new StreamReader(EntryPoint.execAssembly!.GetManifestResourceStream(typeof(EntryPoint), "buildobjects.txt")!))
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
                objects.Value.ForEach(save => { currentData.isUsingModdedObjects = save.modid != "none"; });


            }


            var fileInfo = new FileInfo(Worlds.FullName + "\\" + currentData.nameOfLevel + ".srle");
            var fileStream = fileInfo.Open(FileMode.OpenOrCreate);
            "Saving SRLE Level".LogWarning();
            currentData.Write(fileStream);
            "Completed SRLE Level".LogWarning();

            fileStream.Dispose();
            return true;

        }




        public static int GetRenderId(GameObject objectToGetHashCode)
        {
            MeshFilter[] meshFilters = objectToGetHashCode.GetComponentsInChildren<MeshFilter>();
            Renderer[] renderers = objectToGetHashCode.GetComponentsInChildren<Renderer>();
            string text = meshFilters.Where(meshFilter => meshFilter.sharedMesh is not null).Aggregate("", (current, meshFilter) => current + meshFilter.sharedMesh.name);

            text = renderers.Where(renderer => renderer.material is not null).Aggregate(text, (current, renderer) => current + renderer.material.name);

            if (text == "")
            {
                return 0;
            }

            var num2 = text.GetHashCode();
            return num2 != -1 ? num2 : 0;
        }
        

        internal static void AddModdedObject(GameObject objectToAdd)
        {
            var modInfoName = IntermodCommunication.CallingMod;
          

            if (BuildObjects.ContainsKey("Mods"))
            {
                var name = $"[{modInfoName}] " + objectToAdd.name;
                if (BuildObjects["Mods"].ContainsKey(name)) return;
            }
            
            
            var instantiateInactive = GameObjectUtils.InstantiateInactive(objectToAdd);
            var instantiateInactiveName = $"[{modInfoName}] " + instantiateInactive.name.Replace("(Clone)", string.Empty);
            instantiateInactive.name = instantiateInactiveName;
            instantiateInactive.transform.SetParent(SRLEManager.DontDestroyObjects.transform);

            if (!BuildObjects.ContainsKey("Mods"))
            {
                ModdedIdClass moddedIdClass = new ModdedIdClass {modid = modInfoName, Id = $"mod.1", Name = instantiateInactive.name, Path = instantiateInactive.GetFullName(), HashCode = GetRenderId(objectToAdd)};
                BuildObjects.Add("Mods", new Dictionary<string, IdClass> {{moddedIdClass.Id, moddedIdClass}}); 
            }
            else
            {
                var count = BuildObjects["Mods"].Count;
                count++;

                string idOfSRLE = $"mod.{count}";
                ModdedIdClass moddedIdClass = new ModdedIdClass
                    {modid = modInfoName, Id = idOfSRLE, Name = instantiateInactive.name, Path = instantiateInactive.GetFullName(), HashCode = GetRenderId(objectToAdd)};

                BuildObjects["Mods"].Add(moddedIdClass.Id, moddedIdClass);
            }
            
        }
    }
}