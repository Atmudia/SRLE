global using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using Il2CppSystem;
using Il2CppSystem.Collections;
using Il2CppSystem.IO;
using MelonLoader;
using MelonLoader.TinyJSON;
using SRLE.Patches;
using SRLE.Persistance;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Console = System.Console;
using Exception = System.Exception;
using IEnumerator = System.Collections.IEnumerator;
using Object = Il2CppSystem.Object;

 
namespace SRLE
{

   

    

    public static class Extensions
    {
        public static void Log(this object @this) => MelonLogger.Msg(@this.ToString());
        public static void LogWarning(this object @this) => MelonLogger.Warning(@this.ToString());
        public static void LogError(this object @this) => MelonLogger.Error(@this.ToString());
        public static void PrintComponent(this GameObject obj)
        {
            Log(obj.name);
            foreach (Component component in obj.GetComponentsInChildren<Component>(true))
            {
                Log("   " + component.ToString());
            }
        }
        public static GameObject FindChild(this GameObject obj, string name, bool dive = false)
        {
            if (!dive)
                return obj.transform.Find(name).gameObject;

            GameObject child = null;
            if (obj.transform != null)
                foreach (var o in obj.transform)
                {
                    Transform transform = o.Cast<Transform>();

                    if (!(transform == null))
                    {
                        if (transform.name.Equals(name))
                        {
                            child = transform.gameObject;
                            break;
                        }

                        if (transform.childCount > 0)
                        {
                            child = transform.gameObject.FindChild(name, dive);
                            if (child != null)
                                break;
                        }
                    }
                }

            return child;
        }
        public static string GetFullName(this GameObject obj)
        {
            string str = obj.name;
            for (Transform parent = obj.transform.parent;
                 parent != null;
                 parent = parent.parent)
            {
                str = parent.gameObject.name + "/" + str;
            }

            return str;
        }
        public static bool TryGetResourceLocator(Object key, out IResourceLocator result)
        {
            if (key != null)
            {
                foreach (IResourceLocator resourceLocator in new Il2CppSystem.Collections.Generic.List<IResourceLocator>(Addressables.ResourceLocators).ToArray())
                {
                    
                    var first = resourceLocator.Keys.ToArray().FirstOrDefault(x => x.Equals(key));
                    if (first == null) continue;
                    result = resourceLocator;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public static Dictionary<TK, TV> ToMonoDictionary<TK, TV>(this Il2CppSystem.Collections.Generic.Dictionary<TK, TV> @this)
        {
            Dictionary<TK, TV> dictionary = new Dictionary<TK, TV>();
            foreach (var keyValuePair in @this)
            {
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dictionary;
        }
        public static Il2CppSystem.Collections.Generic.Dictionary<TK, TV> ToIL2CPPDictionary<TK, TV>(this Dictionary<TK, TV> @this)
        {

            Il2CppSystem.Collections.Generic.Dictionary<TK, TV> dictionary =
                new Il2CppSystem.Collections.Generic.Dictionary<TK, TV>();
            foreach (var keyValuePair in @this)
            {
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return dictionary;
            
        }
        public static List<T> ToMonoList<T>(this Il2CppSystem.Collections.Generic.List<T> @this)
        {
            List<T> list = new List<T>();
            foreach (var t in @this)
            {
                list.Add(t);
            }

            return list;
        }
        public static Il2CppSystem.Collections.Generic.List<T> ToIL2CPPList<T>(this System.Collections.Generic.List<T> @this)
        {
            //UnityEngine.GUI.BeginScrollView
            Il2CppSystem.Collections.Generic.List<T> list = new Il2CppSystem.Collections.Generic.List<T>();
            foreach (var t in @this)
            {
                list.Add(t);
            }

            return list;
        }




    }
    public class EntryPoint : MelonMod
    { 
        public const string Version = "1.0.0";

        public override void OnInitializeMelon()
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
          


            var loadFromFile = AssetBundle.LoadFromFile(@"E:\SlimeRancherModding\Unity\SRLE2\Assets\AssetBundles\srle");
            UnityEngine.Object.Instantiate(loadFromFile.LoadAsset("SRLEToolbar").Cast<GameObject>()).hideFlags |=
                HideFlags.HideAndDontSave;






            var directoryInfo = new DirectoryInfo(SRLEMod.SRLEDataPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            ClassInjector.RegisterTypeInIl2Cpp<SRLEMod>();
            var gSRLE = new GameObject("SRLE");
            gSRLE.AddComponent<SRLEMod>();
            gSRLE.hideFlags |= HideFlags.HideAndDontSave;

  

        }

       
        public static List<string> excluded = "GameCore|StandaloneStart|AllZones|XboxOneStart|MainMenu|MainMenuFromBoot|CompanyLogoWithLoadingScreen|UITesting".Split('|').ToList();
        public static GameObject allSceneRelated;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            //sceneName.Log();
            
            switch (sceneName)
            {
                case "MainMenuEnvironment":
                    SRLEMod.IsLoaded = true;
                    SRLEMod.Instance.IsBuildMode = false;
                    
                    break;
                case "GameCore":
                {
                    Patch_Debug.AllSceneGroups = Resources.FindObjectsOfTypeAll<SceneGroup>().Where(sceneGroup => sceneGroup.sceneReferences.Any(reference => Extensions.TryGetResourceLocator(reference.RuntimeKey, out var value)) && excluded.Find(x => x.Contains(sceneGroup.name)) == null).ToArray();
                    break;
                }
                case "SystemCore":
                {
                    allSceneRelated = new GameObject("AllSceneRelated");
                    allSceneRelated.SetActive(true);
                    allSceneRelated.AddComponent<MeshRenderer>();
                    SceneManager.MoveGameObjectToScene(allSceneRelated, SceneManager.GetSceneByName("SystemCore"));
                    break;
                }
                
            }
            

        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName.Equals("MainMenuEnvironment"))
            {
                SRLEMod.IsLoaded = false;
                

            }
        }
    }
}