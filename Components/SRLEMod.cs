
using System;
using System.IO;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace SRLE.Components;

public class SRLEMod : MonoBehaviour
{
    public SRLEMod(IntPtr value) : base(value)
    {
    }
    public enum Mode
    {
        NONE = 0,
        BUILD = 1,
            
    }
    
    public static SRLEMod Instance;
    public static bool IsInMainMenu = false;
    public static Mode CurrentMode;


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Object.Destroy(this);
        MelonLogger.Warning("Tried to create another instance of SRLEMod");
    }

    public void OnGUI()
    {
        if (!IsInMainMenu) return;
        GUI.Label(new Rect(15f, 125f, 150f, 25f), "SRLE v" + EntryPoint.Version);
            
        var files = Directory.GetFiles(SRLESaveManager.DataPath, "*.srle");
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var levelName = Path.GetFileNameWithoutExtension(file);
            if (GUI.Button(new Rect(15f, 150f + 35f * i, 150f, 25f), "Load " + levelName))
            {
               SRLESaveManager.LoadLevel(file);
            }
        }
        currentLevel = GUI.TextField(new Rect(125f, 170f + 35f * files.Length, 200f, 25f), currentLevel);

        GUI.Box(new Rect(15f, 150f + 35f * files.Length, 350f, 100f), "");
        GUI.Label(new Rect(25f, 170f + 35f * files.Length, 150f, 25f), "Level Name:");
        if (GUI.Button(new Rect(25f, 210f + 35f * files.Length, 150f, 25f), "Create new Level"))
        {
            var fileName = currentLevel + ".srle";
            var isValid = !string.IsNullOrEmpty(fileName) &&
                          fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                          !File.Exists(Path.Combine(currentLevel, fileName));
            if (isValid)
            {
                SRLESaveManager.CreateLevel(currentLevel);
            }
            else
            {
                error = "Invalid Levelname or already exists";
            }
        }
        if (!string.IsNullOrWhiteSpace(error))
        {
            MelonLogger.Error(error);
            /*
            GUI.Label(new Rect(25f, 240f + 35f * files.Length, 250f, 25f), "Error creating level: " + error);
            if (GUI.Button(new Rect(290f, 240f + 35f * files.Length, 50f, 25f), "Ok"))
            {
                error = string.Empty;
            }
            */
        }
    }

    public static bool IsSceneLoaderPatch;
    public static string error;
    public string currentLevel;


}