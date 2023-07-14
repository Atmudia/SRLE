using System.IO;
using System.Linq;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppSystem.Collections.Generic;
using SRLE.Components;
using UnityEngine;

namespace SRLE;

public static class SRLESaveManager
{
    public static string DataPath => Path.Combine(Application.dataPath, "..", "SRLE");
    public static string BuildObjectsPath => Path.Combine(Application.dataPath, "..", "SRLE", "BuildObjects.json");
    public static WorldV01 CurrentLevel;

    public static void CreateLevel(string levelName)
    {
        SRLEMod.IsSceneLoaderPatch = true;
        SRLEMod.CurrentMode = SRLEMod.Mode.BUILD;
        CurrentLevel = new WorldV01()
        {
            WorldName = levelName,
            BuildObjects = new System.Collections.Generic.Dictionary<uint, System.Collections.Generic.List<BuildObject>>(),
            Dependencies = new System.Collections.Generic.Dictionary<string, string>(), 
            Path = Path.Combine(DataPath, $"{levelName}.json")
        };
        var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
        {
            saveSlotIndex = 999,
            gameSettingsModel = new GameSettingsModel(new List<OptionsItemDefinition>().Cast<IEnumerable<OptionsItemDefinition>>()),
                    
        };
        loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(Resources.FindObjectsOfTypeAll<GameIconDefinition>().First());
        SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
            () =>
            {
                        
            }));
        
    }

    public static void SaveLevel()
    {
        File.WriteAllText(CurrentLevel.Path, Newtonsoft.Json.JsonConvert.SerializeObject(CurrentLevel));
    }
    public static void LoadLevel(string levelPath)
    {
        
    }

    public static void LoadObjectsFromLevel()
    {
        if (SRLESaveManager.CurrentLevel.BuildObjects.Count == 0) 
            return;
        foreach (var id in CurrentLevel.BuildObjects.Keys)
        {
            var objectFromId = SRLEObjectManager.FindObject(id);
            if (objectFromId == null) continue;
            foreach (var data in CurrentLevel.BuildObjects[id])
            {
                SRLEObjectManager.SpawnObject(id, data);
            }
        }
    }

}