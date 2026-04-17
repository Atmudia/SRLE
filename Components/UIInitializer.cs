using SRLE.RuntimeGizmo;
using UnityEngine;

namespace SRLE.Components
{
    public static class UIInitializer
    {
        public static void Initialize()
        {
            var srleGameObject = LevelManager.SRLEGameObject = new GameObject(nameof(LevelManager.SRLEGameObject));
            srleGameObject.hideFlags |= HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(srleGameObject);
            var srleCamera = LevelManager.SRLEGameObject = new GameObject("SRLECamera")
            {
                transform = { parent = srleGameObject.transform}
            };
            srleCamera.AddComponent<Camera>();
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
        
        }
    }
}