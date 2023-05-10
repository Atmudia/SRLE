global using Il2Cpp;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.Popup;
using Il2CppSystem;
using Il2CppSystem.IO;
using MelonLoader;
using SRLE.Components;
using SRLE.Patches;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using DirectoryInfo = System.IO.DirectoryInfo;
using Object = Il2CppSystem.Object;

 
namespace SRLE
{

    [HarmonyPatch(typeof(DecoInstanceManager), nameof(DecoInstanceManager.OnBeginContextRendering))]
    public class DecoInstanceManagerStart
    {
        
        public static bool Prefix(DecoInstanceManager __instance)
        {
            
            if (__instance.gameObject.GetComponent<BuildObjectId>() || __instance.gameObject.GetComponentInParent<BuildObjectId>() || __instance.gameObject.GetComponentInChildren<BuildObjectId>())
            {
                UnityEngine.Object.Destroy(__instance);
                return false;
            }
            return true;
        }
    }
    
    
    public class EntryPoint : MelonMod
    {
        public const string Version = "1.0.0";
        
        public override void OnInitializeMelon()
        {
            
            var directoryInfo = new DirectoryInfo(SRLEMod.SRLEDataPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            ClassInjector.RegisterTypeInIl2Cpp<SRLEMod>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildObjectId>();
            ClassInjector.RegisterTypeInIl2Cpp<SRLECamera>();

            var gSRLE = new GameObject("SRLE");
            gSRLE.AddComponent<SRLEMod>();
            gSRLE.hideFlags |= HideFlags.HideAndDontSave;
            
            SRLEManager.LoadBuildObjects();


            /*
            SRLESaveSystem.WorldV01 worldV01 = new SRLESaveSystem.WorldV01
            {
                worldName = "Tester",
                buildObjects = new Dictionary<uint, List<SRLESaveSystem.BuildObject>>(),
                dependencies = new Dictionary<string, string>()
                {
                    {"A test mod", "1.0.0"}
                }
            };
            
            var selectMany = SRLEManager.Categories.SelectMany(x => x.Objects).ToArray();
            foreach (var idClass in selectMany)
            {
                if (idClass.Id == selectMany.Length) break;
                if (!worldV01.buildObjects.TryGetValue(idClass.Id, out var list))
                {
                    list = new List<SRLESaveSystem.BuildObject>();
                    worldV01.buildObjects.Add(idClass.Id, list);
                }


                for (int i = 0; i < 1; i++)
                {
                    list.Add(new SRLESaveSystem.BuildObject()
                    {
                        pos = new Vector3(1, 1, 1).ToVector3Save(),
                        rot = new Vector3(1, 1, 1).ToVector3Save(),
                        properties = new Dictionary<string, string>()
                    });
                }
            }
            

            worldV01.SaveToJSON(@"D:\SteamLibrary\steamapps\common\Slime Rancher 2\SRLE\testing.srle");
            */




        }

        
        public static List<string> excluded = "GameCore|StandaloneStart|AllZones|XboxOneStart|MainMenu|MainMenuFromBoot|CompanyLogoWithLoadingScreen|UITesting".Split('|').ToList();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (sceneName)
            {
                case "MainMenuEnvironment":
                    SRLEManager.ClearSRLEData();
                    SRLEMod.IsLoaded = true;
                    SRLEMod.CurrentMode = SRLEMod.Mode.NONE;
                    

                    break;
                case "GameCore":
                {
                    var array = Resources.FindObjectsOfTypeAll<SceneGroup>().Where(sceneGroup => sceneGroup.sceneReferences.Any(reference => reference.RuntimeKey.TryGetResourceLocator(out var value)) && excluded.Find(x => x.Contains(sceneGroup.name)) == null).ToArray();
                    break;
                }
                case "UICore":
                {
                    if (SRLEMod.CurrentMode != SRLEMod.Mode.BUILD) return;
                    
                    
                    var srleCamera = new GameObject(nameof(SRLEManager.SRLEGameObject));
                    srleCamera.hideFlags |= HideFlags.HideAndDontSave;
                    srleCamera.AddComponent<Camera>();
                    srleCamera.AddComponent<SRLECamera>();
                    break;
                }
            }

            if (!sceneName.EndsWith("Core"))
            {
                if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
                {
                    foreach (var directedActorSpawner in Resources.FindObjectsOfTypeAll<DirectedActorSpawner>())
                    {
                        directedActorSpawner.enabled = !directedActorSpawner.enabled;
                    }
                }
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName.Equals("MainMenuEnvironment"))
            {
                SRLEManager.ClearSRLEData();
            }
        }
    }
}