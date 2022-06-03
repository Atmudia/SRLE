using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DebuggingMod;
using HarmonyLib;
using SRLE.SaveSystem;
using SRML;
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



            SRCallbacks.OnMainMenuLoaded += menu => { };
            SRCallbacks.PreSaveGameLoad += context => { };

            SRLEName srleName = new SRLEName();
            srleName.nameOfLevel = "Swiatek";
            srleName.Write(new FileInfo(Worlds.FullName + @"\Testing").Create());
        }
    }
}
     