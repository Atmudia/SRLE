using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using MelonLoader;
using SRLE.Components;
using SRLE.Models;
using UnityEngine;

namespace SRLE;

public static class SaveManager
{
    public static string DataPath => Path.Combine(Application.dataPath, "..", "SRLE");
        public static string LevelsPath => Path.Combine(DataPath, "Levels");
    public static string TexturesDataPath => Path.Combine(DataPath, "Textures");
    public static LevelData CurrentLevel;
    public static SettingsUI.Settings Settings;



    static SaveManager()
    {
        EnsureFoldersExist();
        Melon<SRLE.EntryPoint>.Logger.Msg("Initializing Save Manager");
    }

    private static void EnsureFoldersExist()
    {
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        if (!Directory.Exists(LevelsPath))
        {
            Directory.CreateDirectory(LevelsPath);
        }    
        if (!Directory.Exists(TexturesDataPath))
        {
            Directory.CreateDirectory(TexturesDataPath);
        } 
        if (!File.Exists(Path.Combine(DataPath, "favorites.txt")))
            File.WriteAllText(
                Path.Combine(DataPath, "favorites.txt"),
                JsonSerializer.Serialize(new List<uint>())
            );    }
    public static void LoadLevel(string levelPath)
    {
        LevelManager.SetMode(LevelManager.Mode.BUILD);
        CurrentLevel = JsonSerializer.Deserialize<LevelData>(File.ReadAllText(levelPath));
        CurrentLevel.Path = levelPath;
        var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
        {
            saveSlotIndex = 999,
            gameSettingsModel = new GameSettingsModel(new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>()),
            
        };
        SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
            () =>
            {
                        
            }));
       
        
        //loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(gameIconDefinition);

    }
    public static void CreateLevel(string levelName)
    {
        
        LevelManager.SetMode(LevelManager.Mode.BUILD);
        CurrentLevel = new LevelData()
        {
            LevelName = levelName,
            BuildObjects = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.List<BuildObjectData>>(),
            Dependencies = new System.Collections.Generic.Dictionary<string, string>(), 
            Path = Path.Combine(LevelsPath, $"{levelName}.srle")
        };
        
        var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
        {
            saveSlotIndex = 999,
            gameSettingsModel = new GameSettingsModel(new Il2CppSystem.Collections.Generic.List<OptionsItemDefinition>().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>()),
                    
        };
        SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
            () =>
            {
                        
            }));
    
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
                    Properties = new System.Collections.Generic.Dictionary<string, string>(),
                    //TODO Add here teleports etc;

                });

            }
           
        }
        File.WriteAllText(CurrentLevel.Path, JsonSerializer.Serialize(CurrentLevel)); }
}