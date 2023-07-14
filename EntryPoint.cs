global using Il2Cpp;
using System.IO;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using SRLE.Components;
using SRLE.RuntimeGizmo;
using UnityEngine;

namespace SRLE
{
    
    public class EntryPoint : MelonMod
    {
        
        public const string Version = "1.0.0";
        
        public override void OnInitializeMelon()
        {
            
            ClassInjector.RegisterTypeInIl2Cpp<SRLEMod>();
            ClassInjector.RegisterTypeInIl2Cpp<SRLECamera>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildObjectId>();
            ClassInjector.RegisterTypeInIl2Cpp<TransformGizmo>();

            var directoryInfo = new DirectoryInfo(SRLESaveManager.DataPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            GameObject manager = new GameObject("SRLE");
            manager.AddComponent<SRLEMod>();
            manager.hideFlags |= HideFlags.HideAndDontSave;
            
            SRLEObjectManager.LoadBuildObjects();
            
            
        }
        public override void OnSceneWasInitialized(int level, string sceneName)
        {
            if (sceneName.Equals("MainMenuEnvironment"))
            {
                SRLEMod.IsInMainMenu = true;
                if (SRLECamera.Instance == null) return;
                
                Object.Destroy(SRLECamera.Instance.gameObject);
                Object.Destroy(SRLEManager.World);
                Object.Destroy(SRLEObjectManager.CachedGameObjects);

                SRLESaveManager.CurrentLevel = null;
                SRLEMod.CurrentMode = SRLEMod.Mode.NONE;
            }

            if (sceneName.Equals("UICore"))
            {
                if (SRLEMod.CurrentMode != SRLEMod.Mode.BUILD) return;
                var srleCamera = new GameObject(nameof(SRLEManager.SRLEGameObject));
                srleCamera.hideFlags |= HideFlags.HideAndDontSave;
                srleCamera.AddComponent<Camera>();
                srleCamera.AddComponent<SRLECamera>();
                srleCamera.AddComponent<TransformGizmo>();
                SRLEManager.SRLEGameObject = srleCamera;
                var world = new GameObject(nameof(SRLEManager.World));
                world.hideFlags |= HideFlags.HideAndDontSave;
                SRLEManager.World = world;
                //srleCamera.AddComponent<TransformGizmo>();
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName.Equals("MainMenuEnvironment"))
            {
                SRLEMod.IsInMainMenu = false;
            }       
        }
    }
}