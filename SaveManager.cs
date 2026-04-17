using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SRLE.Components;
using SRLE.Models;
using UnityEngine;

namespace SRLE
{
    public static class SaveManager
    {
        public static string DataPath => Path.Combine(new DirectoryInfo(Application.dataPath).Parent!.FullName, "SRLE");
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
            Directory.CreateDirectory(DataPath);
            Directory.CreateDirectory(LevelsPath);
            Directory.CreateDirectory(TexturesDataPath);

            string favoritesPath = Path.Combine(DataPath, "favorites.txt");
            if (!File.Exists(favoritesPath))
                File.WriteAllText(favoritesPath, JsonConvert.SerializeObject(new List<uint>()));

            string settingsPath = Path.Combine(DataPath, "settings.txt");
            if (!File.Exists(settingsPath))
            {
                Settings = new SettingsUI.Settings
                {
                    EnableFog = true,
                    HighlightMethod = ObjectHighlight.HighlightType.Wireframe,
                    HighlightStrength = 10,
                    RenderDistance = 1000
                };
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(Settings));
            }
            else
            {
                Settings = JsonConvert.DeserializeObject<SettingsUI.Settings>(File.ReadAllText(settingsPath));
            }
        }

        public static void LoadLevel(string levelPath)
        {
            try
            {
                ChunkManager.Clear();
                var levelData = LevelFileHandle.Deserialize(File.ReadAllText(levelPath), levelPath);
                levelData.Path = levelPath;
                CurrentLevel = levelData;
                LevelManager.SetMode(LevelManager.Mode.BUILD);
                SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("srle", Identifiable.Id.HEN, PlayerState.GameMode.CLASSIC, () => { });
            }
            catch (Exception e)
            {
                CurrentLevel = null;
                LevelManager.SetMode(LevelManager.Mode.NONE);
                EntryPoint.ConsoleInstance.Log($"[SRLE] Failed to load level '{levelPath}': {e.Message}");
            }
        }

        public static void CreateLevel(string levelName, WorldType worldType = WorldType.STANDARD)
        {
            ChunkManager.Clear();
            LevelManager.SetMode(LevelManager.Mode.BUILD);
            CurrentLevel = new LevelData
            {
                LevelName = levelName,
                WorldType = worldType,
                BuildObjects = new Dictionary<uint, List<BuildObjectData>>(),
                Dependencies = new Dictionary<string, string>(),
                Path = Path.Combine(LevelsPath, $"{levelName}.srle")
            };
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("srle", Identifiable.Id.HEN, PlayerState.GameMode.CLASSIC, () => { });
        }

        public static void SaveLevel()
        {
            // Build the new data first — don't touch CurrentLevel until we're ready to commit
            var newBuildObjects = new Dictionary<uint, List<BuildObjectData>>();
            foreach (var kvp in ObjectManager.BuildObjects)
            {
                var list = new List<BuildObjectData>();
                foreach (var obj in kvp.Value)
                {
                    if (!ObjectManager.GetBuildObject(obj, out var buildObject))
                    {
                        EntryPoint.ConsoleInstance.Log($"[SRLE] Skipping object '{obj.name}' during save — no BuildObject component.");
                        continue;
                    }
                    list.Add(new BuildObjectData
                    {
                        Pos = obj.transform.position,
                        Rot = obj.transform.eulerAngles,
                        Scale = obj.transform.localScale,
                        Properties = buildObject.GetData()
                    });
                }
                newBuildObjects.Add(kvp.Key, list);
            }

            CurrentLevel.BuildObjects = newBuildObjects;

            // Write to a temp file first; only replace the real file once the write succeeds
            string tempPath = CurrentLevel.Path + ".tmp";
            try
            {
                File.WriteAllText(tempPath, LevelFileHandle.Serialize(CurrentLevel, CurrentLevel.Path));
                if (File.Exists(CurrentLevel.Path))
                    File.Replace(tempPath, CurrentLevel.Path, null);
                else
                    File.Move(tempPath, CurrentLevel.Path);
            }
            catch (Exception e)
            {
                EntryPoint.ConsoleInstance.Log($"[SRLE] Failed to write save file: {e.Message}");
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
