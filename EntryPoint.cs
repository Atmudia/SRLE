global using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using SRLE.RuntimeGizmo;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using DirectoryInfo = System.IO.DirectoryInfo;
using Object = UnityEngine.Object;

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

        public static Shader[] shaders;
        
        public override void OnInitializeMelon()
        {
            /*var manifestResourceStream = MelonAssembly.Assembly.GetManifestResourceStream("SRLE.srle"); 
            byte[] data = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(data, 0, data.Length);
            */
            var assetBundle = AssetBundle.LoadFromFile(@"C:\Users\komik\SRLEGizmo\Assets\AssetBundles\srle");//AssetBundle.LoadFromMemory(data);
            shaders = assetBundle.LoadAllAssets(Il2CppType.Of<Shader>()).Select(delegate(Object o)
            {
                o.hideFlags |= HideFlags.HideAndDontSave;
                return o.Cast<Shader>();
            }).ToArray();
            var directoryInfo = new DirectoryInfo(SRLEMod.SRLEDataPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            ClassInjector.RegisterTypeInIl2Cpp<SRLEMod>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildObjectId>();
            ClassInjector.RegisterTypeInIl2Cpp<SRLECamera>();
            ClassInjector.RegisterTypeInIl2Cpp<TransformGizmo>();

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
                    srleCamera.AddComponent<TransformGizmo>();
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