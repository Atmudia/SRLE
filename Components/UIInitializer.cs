using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Player.CharacterController;
// using RuntimeHandle;
using SRLE.RuntimeGizmo;
// using SRLE.RuntimeGizmo;
using UnityEngine;

namespace SRLE.Components;

public static class UIInitializer
{
    internal static bool IsInitialized;
    

    public static void Initialize()
    {
        IsInitialized = true;
        var srleCamera = LevelManager.SRLEGameObject = new GameObject(nameof(LevelManager.SRLEGameObject));
        srleCamera.hideFlags |= HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(srleCamera);
        srleCamera.AddComponent<Camera>();
        
        // RuntimeTransformHandle.Create(null, HandleType.SCALE);
        // srleCamera.AddComponent<RuntimeTransformHandle>();
        srleCamera.AddComponent<TransformGizmo>();
        srleCamera.AddComponent<SRLECamera>();
        var hierarchyUI = Object.Instantiate(AssetManager.HierarchyUI, srleCamera.transform);
        
        hierarchyUI.AddComponent<HierarchyUI>();
        hierarchyUI.AddComponent<TeleportUI>();
        hierarchyUI.AddComponent<SettingsUI>();
        hierarchyUI.AddComponent<InspectorUI>();
        hierarchyUI.hideFlags |= HideFlags.HideAndDontSave;
        var toolbarUI = Object.Instantiate(AssetManager.ToolbarUI, srleCamera.transform);
        toolbarUI.AddComponent<ToolbarUI>();
        
        // toolbarUI.hideFlags |= HideFlags.HideAndDontSave;
    }
}