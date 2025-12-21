using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppSystem.Linq;
using MelonLoader;
using MelonLoader.Utils;
using SRLE.Components;
using SRLE.Models;
using SRLE.Patches;
using UnityEngine;

namespace SRLE;

public static class SaveManager
{
    public static string DataPath => Path.Combine(MelonEnvironment.UserDataDirectory, "SRLE");
    public static string LevelsPath => Path.Combine(DataPath, "Levels");
    public static string TexturesDataPath => Path.Combine(DataPath, "Textures");
    public static string BuildObjectsPath => Path.Combine(DataPath, "BuildObjects.json");
    public static LevelData CurrentLevel;
    public static SettingsUI.Settings Settings;
    public const int UsedSaveSlotIndex = 9999999;


    static SaveManager()
    {
        EnsureFoldersExist();
        MelonLogger.Msg("Initializing Save Manager");
    }

    private static void EnsureFoldersExist()
    {
        if (!Directory.Exists(DataPath))
            Directory.CreateDirectory(DataPath);
        if (!Directory.Exists(LevelsPath))
            Directory.CreateDirectory(LevelsPath);
        if (!Directory.Exists(TexturesDataPath))
            Directory.CreateDirectory(TexturesDataPath);
         
        if (!File.Exists(Path.Combine(DataPath, "favorites.txt")))
            File.WriteAllText(
                Path.Combine(DataPath, "favorites.txt"),
                JsonSerializer.Serialize(new List<uint>())
            );    
    }
    /*
    public static void DeleteOldGame()
    {
        try
        {
            var autoSaveDirector = GameContext.Instance.AutoSaveDirector;
        
            var summariesToDelete = new List<Summary>();
            foreach (var summary in autoSaveDirector.EnumerateAllSaveGamesIncludingBackups().ToList())
                if(summary.SaveSlotIndex==UsedSaveSlotIndex)
                    summariesToDelete.Add(summary);
            foreach (var summary in summariesToDelete)
            {
                try
                {

                    autoSaveDirector.DeleteGame(summary.Name);
                    autoSaveDirector._storageProvider.DeleteGameData(summary.SaveName);
                }
                catch { }
            }
        }
        catch { }
    }*/
    public static void LoadLevel(string levelPath)
    {
        LevelManager.SetMode(LevelManager.Mode.BUILD);
        CurrentLevel = JsonSerializer.Deserialize<LevelData>(File.ReadAllText(levelPath));
        CurrentLevel.Path = levelPath;
        
        Patch_SceneLoader.LoadObjectsAndGoToSceneGroup(SystemContext.Instance.SceneLoader.DefaultGameplaySceneGroup,new Vector3(541.6444f, 18.6f, 349.3277f), new Vector3(2.5f,231.5f,0));
         try
        {
           // LocationBookmarksUtil.GoToLocationPlayer(SystemContext.Instance.SceneLoader.DefaultGameplaySceneGroup, new Vector3(999,99999f,999), new Vector3(0,0,0));
        }
        catch { }
        //var gameSettingsModel = new GameSettingsModel(new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>());
        //SRSingleton<GameContext>.Instance.AutoSaveDirector.StartNewGame(UsedSaveSlotIndex,gameSettingsModel);
    
    }
    public static void CreateLevel(string levelName, string levelIcon = "gameIcon_TwinSlime")
    {
        
        LevelManager.SetMode(LevelManager.Mode.BUILD);
        CurrentLevel = new LevelData()
        {
            LevelName = levelName,
            LevelIcon = levelIcon,
            GameVersion = Application.version,
            BuildObjects = new Dictionary<uint, List<BuildObjectData>>(),
            Dependencies = new Dictionary<string, string>(), 
            Path = Path.Combine(LevelsPath, $"{levelName}.srle")
        };

        Patch_SceneLoader.LoadObjectsAndGoToSceneGroup(SystemContext.Instance.SceneLoader.DefaultGameplaySceneGroup,new Vector3(541.6444f, 18.6f, 349.3277f),new Vector3(2.5f,231.5f,0));
        try
        {
           // LocationBookmarksUtil.GoToLocationPlayer(SystemContext.Instance.SceneLoader.DefaultGameplaySceneGroup, new Vector3(999,99999f,999), new Vector3(0,0,0));

            //LocationBookmarksUtil.GoToLocationPlayer(SystemContext.Instance.SceneLoader.DefaultGameplaySceneGroup, new Vector3(541.9353f,20.62804f,349.61053f), new Vector3(2.5f,231.5f,0.0f));
        }
        catch { }
        //var gameSettingsModel = new GameSettingsModel(new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>());
        //SRSingleton<GameContext>.Instance.AutoSaveDirector.StartNewGame(UsedSaveSlotIndex,gameSettingsModel);

    }
    public static void SaveLevel()
    {
        CurrentLevel.BuildObjects.Clear();
        foreach (KeyValuePair<uint, Il2CppSystem.Collections.Generic.List<GameObject>> data in ObjectManager.BuildObjects)
        {
            MelonLogger.Msg(data.Key);
            CurrentLevel.BuildObjects.Add(data.Key, new List<BuildObjectData>());
            foreach (var obj in data.Value)
            {
                CurrentLevel.BuildObjects[data.Key].Add(new BuildObjectData()
                {
                    Pos = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localPosition),
                    Rot = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localEulerAngles),
                    Scale = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localScale),
                    Properties = new Dictionary<string, string>(),
                    //TODO Add here teleports etc;

                });

            }
           
        }
        File.WriteAllText(CurrentLevel.Path, JsonSerializer.Serialize(CurrentLevel)); }
}