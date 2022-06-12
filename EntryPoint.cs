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
        

        public static Assembly execAssembly = Assembly.GetExecutingAssembly();
        public override void PreLoad()
        {
        
            


            /*if (!SRLEManager.SRLE.Exists)
            {
                SRLEManager.SRLE.Create();
                SRLEManager.SRLE.CreateSubdirectory("Worlds");
            }
            */

            HarmonyInstance.PatchAll();
            TranslationPatcher.AddUITranslation("l.srle.window_title", "SRLE - Slime Rancher Level Editor");
            TranslationPatcher.AddUITranslation("l.srle.load_a_level", "Load a Level");
            TranslationPatcher.AddUITranslation("l.srle.create_a_level", "Create a Level");
            Console.RegisterCommand(new SRLE_CreateLevelCommand());
            //Console.RegisterCommand(new SRLE_PlaceObjectCommand());
            //Console.RegisterCommand(new SRLE_LoadLevelCommand());

            SRLEManager.LoadObjectsFromBuildObjects();




            SRCallbacks.OnMainMenuLoaded += menu =>
            {

                SRLEManager.currentData = null;
                SRLEManager.isSRLELevel = false;
                
                
            };
            SRCallbacks.PreSaveGameLoad += context =>
            {
                
  
                if (SRLEManager.isSRLELevel)
                {
                    SRLEUIManager.LoadUIData();
                    /*foreach (var currentDataObject in SRLEManager.currentData.objects)
                    {
                        if (SRLEManager.BuildObjects.TryGetValue(currentDataObject.Key, out var @idClass))
                        {
                            foreach (var VARIABLE in currentDataObject.Value)
                            {

                                var instantiateInactive = GameObjectUtils.InstantiateInactive(GameObject.Find(@idClass.Path));
                                instantiateInactive.transform.localPosition = VARIABLE.position.value;
                                instantiateInactive.transform.localRotation = Quaternion.Euler(VARIABLE.rotation.value);
                                instantiateInactive.transform.localScale = VARIABLE.scale.value;
                                instantiateInactive.SetActive(true);

                            }
                        }
                    }
                    */
                }
                
            };
            
        }
    }
}