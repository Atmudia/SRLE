using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DebuggingMod;
using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
using SRLE.SaveSystem;
using SRML;
using SRML.Console;
using SRML.SR;
using SRML.SR.UI.Utils;
using SRML.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;

namespace SRLE
{
    public class EntryPoint : ModEntryPoint
    {

        public override void PreLoad()
        {
            DirectoryInfo SRLE = new DirectoryInfo(Environment.CurrentDirectory + "/SRLE");
            DirectoryInfo Worlds = new DirectoryInfo(Environment.CurrentDirectory + "/SRLE/Worlds");

            if (!SRLE.Exists)
            {
                SRLE.Create();
                SRLE.CreateSubdirectory("Worlds");
            }

            HarmonyInstance.PatchAll();
            TranslationPatcher.AddUITranslation("l.srle.window_title", "SRLE - Slime Rancher Level Editor");
            TranslationPatcher.AddUITranslation("l.srle.load_a_level", "Load a Level");
            TranslationPatcher.AddUITranslation("l.srle.create_a_level", "Create a Level");

            Console.RegisterCommand(new SRLEPlaceObjectsCommand());

            SRLEManager.LoadObjectsFromBuildObjects();


                /*SRLEName srleName = new SRLEName
            {
                nameOfLevel = "SRLETest", 
                objects = new Dictionary<ulong, List<SRLESave>>()
            };*/
            /*var srleSaves = new List<SRLESave>
            {
                new SRLESave(new Vector3(74.1034F, 15.5003f, -63.4266f), new Vector3(0, 1, 0), new Vector3(10, 10, 10))
            };
            
            srleName.objects.Add(64, srleSaves);
            //srleName.objects.Add(1415, srleSaves);
            */


            /*new SRLEName
            {
                nameOfLevel = "SRLETest", 
                objects = new Dictionary<SRLEId, List<SRLESave>>()
            }.Write(new FileInfo(Worlds.FullName + @"\Testing.srle").Open(FileMode.OpenOrCreate));
            */
            
            SRCallbacks.PreSaveGameLoad += menu =>
            {
                SRLEName name = new SRLEName();
                var fileStream = new FileInfo(Worlds.FullName + @"\Testing.srle").Open(FileMode.Open);
                name.Load(fileStream);
                Console.Log(name.nameOfLevel);
                foreach (var VARIABLE in name.objects)
                {
                    Console.Log(VARIABLE.Key.id.ToString());
                    SRLEManager.BuildObjects.TryGetValue(VARIABLE.Key.id, out var idClass);
                    if (idClass != null)
                    {
                        var instantiateInactive = GameObjectUtils.InstantiateInactive(GameObject.Find(idClass.Path));
                        Vector3 position = Vector3.zero;
                        Vector3 rotation = Vector3.zero;
                        Vector3 scale = Vector3.zero;

                        VARIABLE.Value.ForEach(save => position = save.position.value);
                        VARIABLE.Value.ForEach(save => rotation = save.rotation.value);
                        VARIABLE.Value.ForEach(save => scale = save.scale.value);


                        instantiateInactive.transform.position = position;
                        instantiateInactive.transform.rotation = Quaternion.Euler(rotation);
                        instantiateInactive.transform.localScale = scale;
                        
                        instantiateInactive.SetActive(true);

                        
                    }
                    fileStream.Dispose();
                    
                }
                

            };
            
                SRCallbacks.PreSaveGameLoad += context => { };
            
        }
    }
}