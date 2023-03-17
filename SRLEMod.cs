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
    public class SRLEMod : MonoBehaviour
    {
        public static string SRLEDataPath => Path.Combine(Application.dataPath, "..", "SRLE");
        public static string CurrentLevel;
        public bool IsBuildMode;
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

            var files = Directory.GetFiles(SRLEDataPath, "*.world");
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var levelname = Path.GetFileNameWithoutExtension(file);

                if (GUI.Button(new Rect(15f, 150f + 35f * i, 150f, 25f), "Load " + levelname))
                {
                    LoadLevel(true);
                }

                if (GUI.Button(new Rect(170f, 150f + 35f * i, 160f, 25f), "Continue " + levelname))
                {
                    LoadLevel(false);
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
                    LoadLevel(true);
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

        // ReSharper disable Unity.PerformanceAnalysis
        private void LoadLevel(bool buildmode)
        {
            CurrentLevel.Log();
            IsBuildMode = buildmode;
            if (buildmode)
            {
                var loadNewGameMetadata = new AutoSaveDirector.LoadNewGameMetadata
                {
                    saveSlotIndex = 999,
                    gameSettingsModel = new GameSettingsModel(new System.Collections.Generic.List<OptionsItemDefinition>().ToIL2CPPList().Cast<IEnumerable<OptionsItemDefinition>>())
                };

                var firstOrDefault = Resources.FindObjectsOfTypeAll<SceneGroup>().FirstOrDefault(x => x.name.Contains("AllZones"));
                firstOrDefault.coreSceneReference = SRSingleton<SystemContext>.Instance.SceneLoader
                    .defaultGameplaySceneGroup.coreSceneReference;
                
                foreach (var sceneGroup in Resources.FindObjectsOfTypeAll<SceneGroup>())
                {
                    sceneGroup.showLoadingScreen = !sceneGroup.showLoadingScreen;
                }
                
                loadNewGameMetadata.gameSettingsModel.SetGameIconForNewGame(Resources.FindObjectsOfTypeAll<GameIconDefinition>().First(x => x.persistenceId.Equals("gameIcon_pinkSlime")));
                    SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame(loadNewGameMetadata, new System.Action(
                    () =>
                    {
                        
                    }));
                    
            }
        }
    }
}