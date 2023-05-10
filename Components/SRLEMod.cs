using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppMonomiPark.SlimeRancher.UI.Popup;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SRLE.Components
{
    
    
    public class SRLEMod : MonoBehaviour
    {
        public enum Mode
        {
            NONE = 0,
            BUILD = 1,
            PLAY = 2,
            
        }
        
        
        public static string SRLEDataPath => Path.Combine(Application.dataPath, "..", "SRLE");
        public static string CurrentLevel;
        public static string CurrentLevelPath;

        
        public static SceneGroup DefaultZone;

        public static Mode CurrentMode;
        public static SRLEMod Instance;
        public static bool IsLoaded = false;
        private string error;

        public void Awake()
        {
            Instance = this;
        }

        private void OnGUI()
        {
            if (!IsLoaded) return;
            GUI.Label(new Rect(15f, 125f, 150f, 25f), "SRLE v" + EntryPoint.Version);
            
            var files = Directory.GetFiles(SRLEDataPath, "*.srle");
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var levelName = Path.GetFileNameWithoutExtension(file);
                if (GUI.Button(new Rect(15f, 150f + 35f * i, 150f, 25f), "Load " + levelName))
                {
                    LoadLevel(File.ReadAllText(file).LoadFromJSON<SRLESaveSystem.WorldV01>()); 
                }
            }
            CurrentLevel = GUI.TextField(new Rect(125f, 170f + 35f * files.Length, 200f, 25f), CurrentLevel);

            GUI.Box(new Rect(15f, 150f + 35f * files.Length, 350f, 100f), "");
            GUI.Label(new Rect(25f, 170f + 35f * files.Length, 150f, 25f), "Level Name:");
            if (GUI.Button(new Rect(25f, 210f + 35f * files.Length, 150f, 25f), "Create new Level"))
            {
                var fileName = CurrentLevel + ".srle";
                var isValid = !string.IsNullOrEmpty(fileName) &&
                              fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                              !File.Exists(Path.Combine(SRLEDataPath, fileName));
                if (isValid)
                {
                    
                    CreateLevel(CurrentLevel);
                    
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
        
        /*

        private void LoadLevel(string levelName)
        {
            MelonLogger.Msg(levelName);
            CurrentMode = Mode.BUILD;
            var srleLevel = Path.Combine(SRLEDataPath, levelName + ".srle");
            CurrentLevelPath = srleLevel;
            if (File.Exists(srleLevel))
            {
                SRLESaveSystem.LoadLevel(srleLevel);
            }
            else
            {
                SRLESaveSystem.CreateLevel(CurrentLevel);
            }
              
            var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
            {
                saveSlotIndex = 999,
                gameSettingsModel = new GameSettingsModel(new System.Collections.Generic.List<OptionsItemDefinition>().ToIL2CPPList().Cast<IEnumerable<OptionsItemDefinition>>()),
                    
            };
            var sceneLoaderDefaultGameplaySceneGroup = SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup;
            DefaultZone = sceneLoaderDefaultGameplaySceneGroup;

            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup.coreSceneReference = DefaultZone.coreSceneReference;

            loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(Resources.FindObjectsOfTypeAll<GameIconDefinition>().First(x => x.persistenceId.Equals("gameIcon_pinkSlime")));
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
                () =>
                {
                        
                }));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = DefaultZone;
        }
        */

        private void CreateLevel(string levelName)
        {
            MelonLogger.Msg(levelName);
            CurrentMode = Mode.BUILD;
            var srleLevel = Path.Combine(SRLEDataPath, levelName + ".srle");
            CurrentLevelPath = srleLevel;
            SRLESaveSystem.CreateLevel(CurrentLevel);
            
            
            var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
            {
                saveSlotIndex = 999,
                gameSettingsModel = new GameSettingsModel(new System.Collections.Generic.List<OptionsItemDefinition>().ToIL2CPPList().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>()),
                    
            };
            var sceneLoaderDefaultGameplaySceneGroup = SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup;
            DefaultZone = sceneLoaderDefaultGameplaySceneGroup;

            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup.coreSceneReference = DefaultZone.coreSceneReference;

            loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(Resources.FindObjectsOfTypeAll<GameIconDefinition>().First(x => x.persistenceId.Equals("gameIcon_pinkSlime")));
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
                () =>
                {
                        
                }));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = DefaultZone;
            
        }
        private void LoadLevel(SRLESaveSystem.WorldV01 currentLevel)
        {

            var melonMods = MelonMod.RegisteredMelons.ToArray();
            var missingMods = (from modName in currentLevel.dependencies let firstOrDefault = melonMods.FirstOrDefault(x => x.Info.Name == modName.Key && x.Info.Version == modName.Value) where firstOrDefault == null select modName.Key + " : " + modName.Value).ToList();

            if (missingMods.Count != 0)
            {
                error = "Missing dependencies (mods): \n" + missingMods.Aggregate(string.Empty, (current, mod) => current + (mod + Environment.NewLine));
                return;
            }
            MelonLogger.Msg(currentLevel.worldName);
            CurrentMode = Mode.BUILD;
            SRLESaveSystem.CurrentLevel = currentLevel;
            CurrentLevelPath = Path.Combine(SRLEDataPath, CurrentLevel + ".srle");
            var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
            {
                saveSlotIndex = 999,
                gameSettingsModel = new GameSettingsModel(new System.Collections.Generic.List<OptionsItemDefinition>().ToIL2CPPList().Cast<Il2CppSystem.Collections.Generic.IEnumerable<OptionsItemDefinition>>()),
                    
            };
            var sceneLoaderDefaultGameplaySceneGroup = SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup;
            DefaultZone = sceneLoaderDefaultGameplaySceneGroup;

            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup.coreSceneReference = DefaultZone.coreSceneReference;

            loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(Resources.FindObjectsOfTypeAll<GameIconDefinition>().First(x => x.persistenceId.Equals("gameIcon_pinkSlime")));
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
                () =>
                {
                        
                }));
            SRSingleton<SystemContext>.Instance.SceneLoader.defaultGameplaySceneGroup = DefaultZone;


        }

    }
}
