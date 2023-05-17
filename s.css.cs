using System;
using System.IO;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.DataModel;
using Il2CppMonomiPark.SlimeRancher.Options;
using Il2CppMonomiPark.SlimeRancher.SceneManagement;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using SRLE.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRLE
{
    public class SRLEModTest : MonoBehaviour
    {
        public static string SRLEDataPath => Path.Combine(Application.dataPath, "..", "SRLE");
        public static string CurrentLevel;
        public static string CurrentLevelPath;
        public static SceneGroup DefaultZone;

        public bool IsBuildMode = true;
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
                var levelname = Path.GetFileNameWithoutExtension(file);

                if (GUI.Button(new Rect(15f, 150f + 35f * i, 150f, 25f), "Load " + levelname))
                {
                    LoadLevel(levelname);
                }

                if (GUI.Button(new Rect(170f, 150f + 35f * i, 160f, 25f), "Continue " + levelname))
                {
                    LoadLevel(levelname);

                }
            }

            GUI.Box(new Rect(15f, 150f + 35f * files.Length, 350f, 100f), "");
            GUI.Label(new Rect(25f, 170f + 35f * files.Length, 150f, 25f), "Level Name:");
            CurrentLevel = GUI.TextField(new Rect(125f, 170f + 35f * files.Length, 200f, 25f), CurrentLevel);
            if (GUI.Button(new Rect(25f, 210f + 35f * files.Length, 150f, 25f), "Create new Level"))
            {
                var fileName = CurrentLevel + ".srle";
                var isValid = !string.IsNullOrEmpty(fileName) &&
                              fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                              !File.Exists(Path.Combine(SRLEDataPath, fileName));
                if (isValid)
                {
                    LoadLevel(fileName);
                    
                }
                else
                {
                    error = "Invalid Levelname or already exists";
                }
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                GUI.Label(new Rect(25f, 240f + 35f * files.Length, 250f, 25f), "Error creating level: " + error);
                if (GUI.Button(new Rect(290f, 240f + 35f * files.Length, 50f, 25f), "Ok"))
                {
                    error = null;
                }
            }
        }

        private void LoadLevel(string levelName)
        {
            IsBuildMode = true;
            var srleLevel = Path.Combine(SRLEDataPath, levelName + ".srle");
            CurrentLevelPath = srleLevel;
            CurrentLevel = levelName;
            if (File.Exists(srleLevel))
            {
                SRLESaveSystem.LoadLevel(srleLevel);
            }
            else
            {
                "works?".Log();
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
    }
}