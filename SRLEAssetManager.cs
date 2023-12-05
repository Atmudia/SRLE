using UnityEngine;

namespace SRLE;

public static class SRLEAssetManager
{
    public static GameObject ToolbarUI;
    public static GameObject HierarchyUI;
    public static GameObject CategoryButtonPrefab;
    public static GameObject ObjectButtonPrefab;

    
    public static void LoadAssets()
    {
        var assetBundle = AssetBundle.LoadFromFile(@"D:\SlimeRancherModding\UnityProjects\SRLE\Assets\AssetBundles\srlea");
        ToolbarUI = assetBundle.LoadAsset("SRLEToolbar").Cast<GameObject>();
        ToolbarUI.hideFlags |= HideFlags.HideAndDontSave;
        HierarchyUI = assetBundle.LoadAsset("SRLEHierarchy").Cast<GameObject>();
        HierarchyUI.hideFlags |= HideFlags.HideAndDontSave;
        CategoryButtonPrefab = assetBundle.LoadAsset("CategoryButton").Cast<GameObject>();
        CategoryButtonPrefab.hideFlags |= HideFlags.HideAndDontSave;
        ObjectButtonPrefab = assetBundle.LoadAsset("ObjectButton").Cast<GameObject>();
        ObjectButtonPrefab.hideFlags |= HideFlags.HideAndDontSave;

        
        
    }
}