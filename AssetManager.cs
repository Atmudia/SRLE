﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

namespace SRLE;

public static class AssetManager
{
    public static GameObject ToolbarUI;
    public static GameObject HierarchyUI;
    public static GameObject CategoryButtonPrefab;
    public static GameObject ObjectButtonPrefab;
    
    public static GameObject InspectorVector3;
    public static GameObject InspectorInput;
    public static GameObject InspectorArray;
    public static GameObject InspectorBool;


    public static Material HandleOpaqueMaterial;
    public static Material HandleTransparentMaterial;
    public static Material HandleRotateMaterial;

    public static Material HighlightMaterial;
    public static Material UnlitVertexColorMaterial;
    public static Material WireframeMaterial;

    public static Material Lines;

    public static Mesh ConeMesh;   // Used as translation handle cone caps.
    public static Mesh CubeMesh;    // Used for scale handle
    public static Shader HandleShader;
    public static Shader AdvancedHandleShader;

    public static Material SuperRaycastMaterial;

    public static Dictionary<string, Mesh> SeperatedMeshes = new Dictionary<string, Mesh>();


    static AssetManager()
    {
        MelonCoroutines.Start(LoadAssetBundleData());
    }

    public static T LoadAsset<T>(this AssetBundle @this, string name) where T : Il2CppObjectBase => @this.LoadAsset(name).Cast<T>();

    private static IEnumerator LoadAssetBundleData()
    {
        Melon<EntryPoint>.Logger.Msg("Initializing Asset Manager");
        AssetBundle assetBundle;
        using (Stream bundleStream = Melon<EntryPoint>.Instance.MelonAssembly.Assembly.GetManifestResourceStream("SRLE.srle"))
        {
            byte[] bundleBytes = new byte[bundleStream.Length];
            _ = bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
            assetBundle = AssetBundle.LoadFromMemory(bundleBytes);
        }
        ToolbarUI = assetBundle.LoadAsset<GameObject>("BetterBuildToolbar");
        ToolbarUI.hideFlags |= HideFlags.HideAndDontSave;
        HierarchyUI = assetBundle.LoadAsset<GameObject>("BetterBuildHierarchy");
        HierarchyUI.hideFlags |= HideFlags.HideAndDontSave;
        CategoryButtonPrefab = assetBundle.LoadAsset<GameObject>("CategoryButton");
        CategoryButtonPrefab.hideFlags |= HideFlags.HideAndDontSave;
        ObjectButtonPrefab = assetBundle.LoadAsset<GameObject>("ObjectButton");
        ObjectButtonPrefab.hideFlags |= HideFlags.HideAndDontSave;
        InspectorVector3 = assetBundle.LoadAsset<GameObject>("InspectorVector3");
        InspectorVector3.hideFlags |= HideFlags.HideAndDontSave;
        InspectorInput = assetBundle.LoadAsset<GameObject>("InspectorInput");
        InspectorInput.hideFlags |= HideFlags.HideAndDontSave;
        InspectorArray = assetBundle.LoadAsset<GameObject>("InspectorArray");
        InspectorArray.hideFlags |= HideFlags.HideAndDontSave;
        InspectorBool = assetBundle.LoadAsset<GameObject>("InspectorBool");
        InspectorBool.hideFlags |= HideFlags.HideAndDontSave;

        ConeMesh = assetBundle.LoadAsset<Mesh>("ConeSoftEdges");
        ConeMesh.hideFlags |= HideFlags.HideAndDontSave;
        CubeMesh = assetBundle.LoadAsset<Mesh>("Cube");
        CubeMesh.hideFlags |= HideFlags.HideAndDontSave;

        HandleOpaqueMaterial = assetBundle.LoadAsset<Material>("HandleOpaqueMaterial");
        HandleOpaqueMaterial.hideFlags |= HideFlags.HideAndDontSave;
        HandleRotateMaterial = assetBundle.LoadAsset<Material>("HandleRotateMaterial");
        HandleRotateMaterial.hideFlags |= HideFlags.HideAndDontSave;
        HandleTransparentMaterial = assetBundle.LoadAsset<Material>("HandleTransparentMaterial");
        HandleTransparentMaterial.hideFlags |= HideFlags.HideAndDontSave;

        HighlightMaterial = assetBundle.LoadAsset<Material>("Highlight");
        HighlightMaterial.hideFlags |= HideFlags.HideAndDontSave;
        UnlitVertexColorMaterial = assetBundle.LoadAsset<Material>("UnlitVertexColor");
        UnlitVertexColorMaterial.hideFlags |= HideFlags.HideAndDontSave;
        WireframeMaterial = assetBundle.LoadAsset<Material>("Wireframe");
        WireframeMaterial.hideFlags |= HideFlags.HideAndDontSave;
        Lines = assetBundle.LoadAsset<Material>("Lines");
        Lines.hideFlags |= HideFlags.HideAndDontSave;

       
        HandleShader = assetBundle.LoadAsset<Shader>("assets/betterbuild/anothergizmo/handleshader.shader");
        HandleShader.hideFlags |= HideFlags.HideAndDontSave;
        AdvancedHandleShader = assetBundle.LoadAsset<Shader>("assets/betterbuild/anothergizmo/handleshader.shader");
        AdvancedHandleShader.hideFlags |= HideFlags.HideAndDontSave;

        // SuperRaycastMaterial = assetBundle.LoadAsset<Material>("Super Raycast");
        // SuperRaycastMaterial.hideFlags |= HideFlags.HideAndDontSave;
        
        using (Stream bundleStream = Melon<EntryPoint>.Instance.MelonAssembly.Assembly.GetManifestResourceStream("SRLE.srlemeshes"))
        {
            byte[] bundleBytes = new byte[bundleStream.Length];
            _ = bundleStream.Read(bundleBytes, 0, bundleBytes.Length);
            var srlemeshes = AssetBundle.LoadFromMemory(bundleBytes);
            foreach (var loadAllAsset in srlemeshes.LoadAllAssets(Il2CppType.Of<Mesh>()))
            {
                loadAllAsset.hideFlags |= HideFlags.HideAndDontSave;
                SeperatedMeshes.Add(loadAllAsset.name, loadAllAsset.Cast<Mesh>());
            }
        }
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (ToolbarUI == null)
        {
            Melon<EntryPoint>.Logger.Warning("AssetBundle failed to load.");
        }
        assetBundle.Unload(false);
        
    }
}