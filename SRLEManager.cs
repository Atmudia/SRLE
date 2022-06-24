using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SRLE.SaveSystem;
using SRML.Console;
using UnityEngine;

namespace SRLE
{
    public static class SRLEManager
    {
        public static Dictionary<string, Dictionary<ulong, IdClass>> BuildObjects = new Dictionary<string, Dictionary<ulong, IdClass>>();
        public static SRLEName currentData;
        public static bool isSRLELevel;
        public static DirectoryInfo Worlds = new DirectoryInfo(Environment.CurrentDirectory + "/SRLE/Worlds");


        internal static void LoadObjectsFromBuildObjects()
        {
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(File.ReadAllText(@"E:\SteamLibrary\steamapps\common\Slime Rancher\SRLE\BuildObjects\buildobjects.txt"));
            foreach (var category in categories)
            {
                foreach (var idClass in category.Objects)
                {
                    if (!BuildObjects.ContainsKey(category.CategoryName))
                        BuildObjects.Add(category.CategoryName, new Dictionary<ulong, IdClass>
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
                /*if (objects.Value.modid != "none")
                {
                    currentData.ifUsingModdedObjects = true;
                }
                */

            }

            
            var fileInfo = new FileInfo(Worlds.FullName + "\\" + currentData.nameOfLevel + ".srle");
            var fileStream = fileInfo.Open(FileMode.OpenOrCreate);
            "Saving SRLE Level".LogWarning();
            currentData.Write(fileStream);
            "Completed SRLE Level".LogWarning();

            fileStream.Dispose();
            return true;
        }

        public static void AddModdedObject(GameObject gameObject)
        {
            //throw new System.NotImplementedException();
        }
    }
}