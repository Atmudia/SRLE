global using Il2Cpp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using SRLE.RuntimeGizmo;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.HighDefinition.Compositor;
using UnityEngine.SceneManagement;
using UnityEngine.VFX.SDF;
using Object = UnityEngine.Object;

namespace SRLE
{
  
    public class EntryPoint : MelonMod
    {

        public const string Version = "1.0.0";

        public static SceneGroup VoidGroup;


        public override void OnInitializeMelon()
        {
                ClassInjector.RegisterTypeInIl2Cpp<SRLEMod>();
            ClassInjector.RegisterTypeInIl2Cpp<SRLECamera>();
            ClassInjector.RegisterTypeInIl2Cpp<BuildObjectId>();
            ClassInjector.RegisterTypeInIl2Cpp<TransformGizmo>();
            ClassInjector.RegisterTypeInIl2Cpp<ToolbarUI>();
            ClassInjector.RegisterTypeInIl2Cpp<HierarchyUI>();
            ClassInjector.RegisterTypeInIl2Cpp<TeleportUI>();
            ClassInjector.RegisterTypeInIl2Cpp<SettingsUI>();


            var directoryInfo = new DirectoryInfo(SRLESaveManager.DataPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            GameObject manager = new GameObject("SRLE");
            manager.AddComponent<SRLEMod>();
            manager.hideFlags |= HideFlags.HideAndDontSave;

            SRLEObjectManager.LoadBuildObjects();

            VoidGroup = ScriptableObject.CreateInstance<SceneGroup>();
            VoidGroup.hideFlags |= HideFlags.HideAndDontSave;

        }

        public override void OnSceneWasInitialized(int level, string sceneName)
        {
            if (SRLEAssetManager.HierarchyUI == null)
                SRLEAssetManager.LoadAssets();
            switch (sceneName)
            {
                case "GameCore":
                {
                    var sceneGroupDefault = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.ReferenceId.Equals("SceneGroup.ConservatoryFields"));
                    VoidGroup._coreSceneReference = sceneGroupDefault.CoreSceneReference;
                    VoidGroup._requiresGameCore = true;
                    VoidGroup._requiresZoneCore = true;
                    VoidGroup._showLoadingScreen = true;
                    VoidGroup._isGameplay = true;
                    VoidGroup._quadTreeBounds = sceneGroupDefault.QuadTreeInitializationValues;
                    var assetReference = new AssetReference
                    {
                        m_AssetGUID = "VoidWorld"
                    };
                    VoidGroup._sceneReferences = new List<AssetReference>
                    {
                        { assetReference }
                    }.ToArray();
                    break;
                }
                case "MainMenuEnvironment":
                {
                    var sceneObj = GameObject.Find("Scene");
                    var renderImage = sceneObj.RenderImage(new RuntimePreviewGeneratorAidanNotworking.RenderConfig(1024, 1024, Camera.main.transform.rotation)
                    {
                        backgroundColor = Color.red
                    }, out var ex,copyObjectForRender:false);
                    File.WriteAllBytes("D:\\SteamLibrary\\steamapps\\common\\Slime Rancher 2\\SRLE\\test.png", renderImage.EncodeToPNG());
                    RuntimePreviewGenerator.MarkTextureNonReadable = false;
                    RuntimePreviewGenerator.BackgroundColor = Color.clear;
                    // var generateModelPreview = RuntimePreviewGenerator.GenerateModelPreview(sceneObj.transform, 1024, 1024);
                    // File.WriteAllBytes("D:\\SteamLibrary\\steamapps\\common\\Slime Rancher 2\\SRLE\\test1.png", generateModelPreview.EncodeToPNG());
                    // var test = CreateTextureFromCamera(sceneObj);
                    //
                    // File.WriteAllBytes("D:\\SteamLibrary\\steamapps\\common\\Slime Rancher 2\\SRLE\\test2.png", test.EncodeToPNG());

                    
                    

                    SRLEMod.IsInMainMenu = true; 
                    if (SRLECamera.Instance == null) return;

                    Object.Destroy(SRLECamera.Instance.gameObject);
                    Object.Destroy(SRLEManager.World);
                    Object.Destroy(SRLEObjectManager.CachedGameObjects);

                    SRLESaveManager.CurrentLevel = null;
                    SRLEMod.CurrentMode = SRLEMod.Mode.NONE;





                    break;
                }
                case "UICore" when SRLEMod.CurrentMode != SRLEMod.Mode.BUILD:
                    return;
                //srleCamera.AddComponent<TransformGizmo>();
                case "UICore":
                {

                    var srleCamera = new GameObject(nameof(SRLEManager.SRLEGameObject));
                    srleCamera.hideFlags |= HideFlags.HideAndDontSave;
                    srleCamera.AddComponent<Camera>();
                    srleCamera.AddComponent<TransformGizmo>();
                    srleCamera.AddComponent<SRLECamera>();
                    var hierarchyUI = Object.Instantiate(SRLEAssetManager.HierarchyUI);
                    hierarchyUI.AddComponent<HierarchyUI>();
                    hierarchyUI.AddComponent<TeleportUI>();
                    hierarchyUI.AddComponent<SettingsUI>();
                    hierarchyUI.hideFlags |= HideFlags.HideAndDontSave;
                    var toolbarUI = Object.Instantiate(SRLEAssetManager.ToolbarUI);
                    toolbarUI.AddComponent<ToolbarUI>();
                    toolbarUI.hideFlags |= HideFlags.HideAndDontSave;

                   
                    
                    
                    SRLEManager.SRLEGameObject = srleCamera;
                    var world = new GameObject(nameof(SRLEManager.World));
                    world.hideFlags |= HideFlags.HideAndDontSave;
                    SRLEManager.World = world;
                    break;
                }
            }
        }

        public static Texture2D CreateTextureFromCamera(GameObject objects)
        {
            // Create a new camera to use for rendering
            Camera cam = new GameObject("CameraForTextureCapture").AddComponent<Camera>();
            cam.gameObject.SetActive(false); // Disable the camera to prevent rendering in the scene

            // Calculate the bounds that encompass the specified object and its children
            Bounds combinedBounds = new Bounds();
            Renderer[] renderers = objects.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            // Calculate the camera's position and orthographic size based on the combined bounds
            float camSizeY = combinedBounds.extents.y + 0.1f;
            float camSizeX = camSizeY * (Screen.width / (float)Screen.height);
            cam.orthographicSize = Mathf.Max(camSizeY, camSizeX);
            cam.transform.position = combinedBounds.center - (objects.transform.rotation * new Vector3(-1f, -1f, -1f).normalized);

            int resX = Screen.width;
            int resY = Screen.height;

            int clipX = 0;
            int clipY = 0;

            if (resX > resY)
                clipX = resX - resY;
            else if (resY > resX)
            {
                clipY = resY - resX;
            }

            // Create a texture and a render texture for rendering
            Texture2D tex = new Texture2D(resX - clipX, resY - clipY, TextureFormat.RGB24, false);
            RenderTexture rt = new RenderTexture(resX, resY, 24);
            cam.targetTexture = rt;
            RenderTexture.active = rt;

            // Render the scene with the adjusted camera
            cam.Render();
            tex.ReadPixels(new Rect(clipX / 2, clipY / 2, resX - clipX, resY), 0, 0);
            tex.Apply(false, false);

            // Clean up and destroy the temporary camera and render texture
            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(rt);
            Object.Destroy(cam.gameObject);

            return tex;
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (sceneName.Equals("MainMenuEnvironment"))
            {
                SRLEMod.IsInMainMenu = false;
            }
        }

        public static void PrintAllShaderProperties(Material mat)
        {
            Shader shader = mat.shader;
            

            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                string propertyName = shader.GetPropertyName(i);
                ShaderPropertyType propertyType = shader.GetPropertyType(i);

                switch (propertyType)
                {
                    case ShaderPropertyType.Color:
                        Color colorProperty = mat.GetColor(propertyName);
                        MelonLogger.Msg($"Color: {propertyName}: R:{colorProperty.r}, G:{colorProperty.g}, B:{colorProperty.b}, A:{colorProperty.a}");
                        break;

                    case ShaderPropertyType.Texture:
                        Texture textureProperty = mat.GetTexture(propertyName);
                        if (textureProperty != null)
                            MelonLogger.Msg($"Texture: {propertyName}: {textureProperty.ToString()}");
                        else
                            MelonLogger.Msg($"Texture: {propertyName} is null");
                        break;

                    case ShaderPropertyType.Float:
                        float floatProperty = mat.GetFloat(propertyName);
                        MelonLogger.Msg($"Float: {propertyName}: {floatProperty}");
                        break;
                    case ShaderPropertyType.Int:
                        float intProperty = mat.GetInt(propertyName);
                        MelonLogger.Msg($"Int: {propertyName}: {intProperty}");
                        break;

                    // Add cases for other property types as needed

                    default:
                        //Debug.Log($"{propertyName}: Unsupported property type");
                        break;
                }
            }
        }
        
    }
}