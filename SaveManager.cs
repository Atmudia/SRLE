using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher;
using MonomiPark.SlimeRancher.DataModel;
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.Models;
using UnityEngine;

namespace SRLE
{
    public static class SaveManager
    {
        public static string DataPath => Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "SRLE");
        public static string LevelsPath => Path.Combine(DataPath, "Levels");
        public static string TexturesDataPath => Path.Combine(DataPath, "Textures");
        public static LevelData CurrentLevel;
        public static SettingsUI.Settings Settings;
        
        static SaveManager()
        {
            EnsureFoldersExist();
            EntryPoint.ConsoleInstance.Log("Initializing Save Manager");
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
                    JsonConvert.SerializeObject(new List<uint>())
                );    
            if(!File.Exists(Path.Combine(SaveManager.DataPath, "settings.txt")))
            {
                SaveManager.Settings = new SettingsUI.Settings()
                {
                    EnableFog = true,
                    HighlightMethod = ObjectHighlight.HighlightType.Wireframe,
                    HighlightStrength = 10,
                    RenderDistance = 1000
                };
            }
            else SaveManager.Settings = JsonConvert.DeserializeObject<SettingsUI.Settings>(File.ReadAllText(Path.Combine(SaveManager.DataPath, "settings.txt")));

        }
        public static void LoadLevel(string levelPath)
        {
            LevelManager.SetMode(LevelManager.Mode.BUILD);
            CurrentLevel = JsonConvert.DeserializeObject<LevelData>(File.ReadAllText(levelPath));
            CurrentLevel.Path = levelPath;
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("srle", Identifiable.Id.HEN, PlayerState.GameMode.CLASSIC, () => {});
        }
        public static void CreateLevel(string levelName)
        {
            LevelManager.SetMode(LevelManager.Mode.BUILD);
            CurrentLevel = new LevelData()
            {
                LevelName = levelName,
                BuildObjects = new Dictionary<uint, List<BuildObjectData>>(),
                Dependencies = new Dictionary<string, string>(), 
                Path = Path.Combine(LevelsPath, $"{levelName}.srle")
            };
        
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("srle", Identifiable.Id.HEN, PlayerState.GameMode.CLASSIC, () => {});

    
        }
        public static void SaveLevel()
        {
            CurrentLevel.BuildObjects.Clear();
            foreach (KeyValuePair<uint, System.Collections.Generic.List<GameObject>> data in ObjectManager.BuildObjects)
            {
                EntryPoint.ConsoleInstance.Log(data.Key);
                CurrentLevel.BuildObjects.Add(data.Key, new List<BuildObjectData>());
                foreach (var obj in data.Value)
                {
                    ObjectManager.GetBuildObject(obj, out var buildObject);
                    CurrentLevel.BuildObjects[data.Key].Add(new BuildObjectData()
                    {
                        Pos = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localPosition),
                        Rot = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localEulerAngles),
                        Scale = BuildObjectData.Vector3Save.ToVector3Save(obj.transform.localScale),
                        Properties = buildObject.GetData()

                    });

                }
           
            }
            File.WriteAllText(CurrentLevel.Path, JsonConvert.SerializeObject(CurrentLevel)); }
    }
}