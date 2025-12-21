global using Il2Cpp;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MelonLoader;
using SR2E;
using SR2E.Expansion;
using SR2E.Utils;
using SRLE;
using SRLE.Components;
using SRLE.Patches;
using UnityEngine;

namespace SRLE
{
    public class EntryPoint : SR2EExpansionV3
    {
        internal static EntryPoint instance;

        internal static string InfoText = "";
        //public const string Version = "1.0.0";
        
        public override void OnInitializeMelon()
        {
            instance = this;
            foreach (var type in AccessTools.GetTypesFromAssembly(MelonBase.MelonAssembly.Assembly))
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            
            var prefs = MelonPreferences.CreateCategory("SRLE", "SRLE");
            if (!prefs.HasEntry("showSRLEModUI")) 
                prefs.CreateEntry("showSRLEModUI", (bool)false, "Show SRLE OnGui in Main Menu", false).AddAction((System.Action)(() =>
                {
                    SRLEMod.showSRLEModUI = prefs.GetEntry<bool>("showSRLEModUI").Value;
                }));
            SRLEMod.showSRLEModUI = prefs.GetEntry<bool>("showSRLEModUI").Value;
        }

        public override void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 30;
            style.alignment = TextAnchor.LowerRight;

            style.normal.textColor = Color.black;
            Vector2 offset = new Vector2(1, 1);
            Rect rect = new Rect(Screen.width - 10 - 200, Screen.height - 30, 200, 30);

            GUI.Label(new Rect(rect.x - offset.x, rect.y, rect.width, rect.height), InfoText, style);
            GUI.Label(new Rect(rect.x + offset.x, rect.y, rect.width, rect.height), InfoText, style);
            GUI.Label(new Rect(rect.x, rect.y - offset.y, rect.width, rect.height), InfoText, style);
            GUI.Label(new Rect(rect.x, rect.y + offset.y, rect.width, rect.height), InfoText, style);

            style.normal.textColor = Color.white;
            GUI.Label(rect, InfoText, style);
        }

        public override void OnLoadSceneLoad()
        {
            // We need to wait it bit because the LoadingScreen doesn't like instantly changing to another scene
            ActionsEUtil.ExecuteInTicks(()=> Patch_SceneLoader.OnSceneLoaded(),3);
        }
    }
}


namespace System.Runtime.CompilerServices
{
    public class IsUnmanagedAttribute : Attribute {
    } 
}