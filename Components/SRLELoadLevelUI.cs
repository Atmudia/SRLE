using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using rail;
using SRLE.SaveSystem;
using SRML.SR;
using SRML.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;

namespace SRLE.Components
{

    internal class SRLELoadLevelUI : BaseUI
    {
        public List<SRLEName> availLevels = new List<SRLEName>();
        public List<Toggle> levelToggles = new();
        public GameObject noSavesPanel;
        public GameObject loadingPanel;
        public GameObject summaryPanel;
        public GameObject loadButtonPanel;
        public GameObject loadGameButtonPrefab;
        public GameObject deleteUIPrefab;
        private bool settingToggleStates;
        private int selectedIdx;
        public ScrollRect scroller;

        public override void Awake()
        {
            

            base.Awake();
            UpdateAvailLevels();
        }
        
        

        public void UpdateAvailLevels()
        {
            foreach (Toggle gameToggle in this.levelToggles)
                Destroyer.Destroy((UnityEngine.Object) gameToggle.gameObject, "SRLELoadLevelUI.UpdateAvailLevels");
            this.levelToggles.Clear();
            availLevels.Clear();
            this.noSavesPanel.SetActive(true);
            this.loadingPanel.SetActive(true);

            StartCoroutine(FinishUpdateAvailGames());
        }

        public IEnumerator FinishUpdateAvailGames()
        {
            
            SRLELoadLevelUI loadGameUi = this;
            yield return (object) new WaitForSeconds(0.0f);
            loadGameUi.availLevels.Clear();
            foreach (KeyValuePair<string, SRLEName> keyValuePair in SRLESaveManager.AvailableGames())
                loadGameUi.availLevels.Add(keyValuePair.Value);
            loadGameUi.loadingPanel.SetActive(false);
            loadGameUi.summaryPanel.gameObject.SetActive(loadGameUi.availLevels.Count > 0);
            loadGameUi.noSavesPanel.gameObject.SetActive(loadGameUi.availLevels.Count <= 0);
            foreach (SRLEName availGame in loadGameUi.availLevels)
            {
                
                GameObject loadGameButton = loadGameUi.CreateLoadGameButton(availGame);
                loadGameButton.transform.SetParent(loadGameUi.loadButtonPanel.transform, false);
                loadGameUi.levelToggles.Add(loadGameButton.GetComponent<Toggle>());
            }

            if (loadGameUi.levelToggles.Count > 0)
                loadGameUi.levelToggles[0].gameObject.AddComponent<InitSelected>();

            for (int index = 0; index < loadGameUi.levelToggles.Count; ++index)
            {
                Navigation navigation = new Navigation {mode = Navigation.Mode.Explicit};
                if (index > 0)
                    navigation.selectOnUp = (Selectable) loadGameUi.levelToggles[index - 1];
                if (index < loadGameUi.levelToggles.Count - 1)
                    navigation.selectOnDown = (Selectable) loadGameUi.levelToggles[index + 1];
                loadGameUi.levelToggles[index].navigation = navigation;
                loadGameUi.AddToggleListener(index);
            }

            if (loadGameUi.availLevels.Count > 0)
                loadGameUi.SetSelectedIdx(0);
            loadGameUi.StartCoroutine(loadGameUi.ScrollToTop());
        }

        public IEnumerator ScrollToTop()
        {
            yield return (object) new WaitForEndOfFrame();
            this.scroller.verticalNormalizedPosition = 1f;
        }

        public GameObject CreateLoadGameButton(SRLEName gameSummary)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.loadGameButtonPrefab);
            Toggle toggle = gameObject.GetComponent<Toggle>();
            toggle.group = this.loadButtonPanel.GetComponent<ToggleGroup>();

            var gameSummaryEntry = gameObject.GetComponent<GameSummaryEntry>();
            
            
            var levelSummaryEntry = gameObject.AddComponent<LevelSummaryEntry>();
            levelSummaryEntry.gameNameText = gameSummaryEntry.gameNameText;
            levelSummaryEntry.gameIcon = gameSummaryEntry.gameIcon;

            levelSummaryEntry.Init(gameSummary);
            Object.DestroyImmediate(gameSummaryEntry);
            OnSelectDelegator.Create(gameObject, (UnityAction) (() => toggle.isOn = true));
            return gameObject;
        }

        public void AddToggleListener(int idx) => this.levelToggles[idx].onValueChanged.AddListener(
            (UnityAction<bool>) (isOn =>
            {
                if (!isOn || this.settingToggleStates)
                    return;
                this.SetSelectedIdx(idx);
            }));

        public void DeleteSelectedLevel()
        {
            {

                this.gameObject.SetActive(false);
                var levelSummary = availLevels[selectedIdx];
                CreateDeleteGameDialog(levelSummary, () =>
                {
                    new FileInfo(Path.Combine(SRLEManager.Worlds.FullName, levelSummary.nameOfFile)).Delete();
                    this.gameObject.SetActive(true);
                }, () =>
                {
                    this.gameObject.SetActive(true);
                    UpdateAvailLevels();

                });

            }
        }
        public void PlaySelectedLevel()
        {
            var levelSummary = availLevels[selectedIdx];
            SRLEManager.currentData = levelSummary;
            SRLEManager.isSRLELevel = true;
            levelSummary.worldType = WorldType.STANDARD;
            SRCallbacks.PreSaveGameLoad += context =>
            {
                switch (levelSummary.worldType)
                {
                    case WorldType.STANDARD:
                        break;
                    case WorldType.SEA:
                    {
                        FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        break;
                    }
                    case WorldType.VOID:
                    {
                        FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);    
                        }

                        break;
                    }
                    case WorldType.DESERT:
                    {
                        FindObjectsOfType<Region>().ToList().ForEach(x => x.gameObject.SetActive(false));
                        foreach (Transform o in GameObject.Find("zoneSEA").transform)
                        {
                            o.gameObject.SetActive(false);    

                        }

                        foreach (Transform o in GameObject.Find("zoneDESERT").transform)
                        {
                            if (o.gameObject.name == "SandSea")
                            {
                                o.transform.SetPositionAndRotation(new Vector3(0, 0,0), Quaternion.identity);
                                o.gameObject.SetActive(true);
                                o.GetComponent<ManageWithRegionSet>().setId = RegionRegistry.RegionSetId.HOME;
                            }
                        }

                        break;
                    }
                }

                foreach (var levelSummaryObject in levelSummary.objects)
                {

                    IdClass idClass = null;
                    foreach (var keyValuePair in SRLEManager.BuildObjects)
                    {
                        if (keyValuePair.Value.TryGetValue(@levelSummaryObject.Key, out var idClass1))
                        {
                            idClass = idClass1;
                            break;
                        }
                    }
                    foreach (var srleSave in levelSummaryObject.Value)
                    {
                       
                        var original = SRSingleton<ContainersOfObject>.Instance.GetObject(idClass.Id);
                        if (original is null) continue;

                        
                        var instantiateInactive = GameObjectUtils.InstantiateInactive(original).transform;
                        foreach (var VARIABLE in srleSave.dictionaryWithProperties)
                        {
                            switch (VARIABLE.Key)
                            {
                                
                                case "JournalText":
                                    instantiateInactive.GetComponent<JournalEntry>().entryKey = "srle." + VARIABLE.Value.property;
                                    break;
                                case "TeleportDestination":
                                    instantiateInactive.GetComponentInChildren<TeleportDestination>().teleportDestinationName = VARIABLE.Value.property;
                                    break;
                                case "TeleportSource":
                                    instantiateInactive.GetComponentInChildren<TeleportSource>().destinationSetName = VARIABLE.Value.property;
                                    break;
                                
                            }   
                        }
                        
                        instantiateInactive.localPosition = srleSave.position.value;
                        instantiateInactive.localEulerAngles = srleSave.rotation.value;
                        instantiateInactive.transform.localScale = srleSave.scale.value;
                        instantiateInactive.gameObject.AddComponent<ObjectAddedBySRLE>().id = idClass.Id;
                        instantiateInactive.gameObject.SetActive(true);
                        
                        






                    }
                }
            };
            SRSingleton<GameContext>.Instance.AutoSaveDirector.LoadNewGame("", Identifiable.Id.HEN, PlayerState.GameMode.CASUAL, () => {});
            

        }
        public GameObject CreateDeleteGameDialog(
            SRLEName srleName,
            ConfirmUI.OnConfirm onConfirm,
            BaseUI.OnDestroyDelegate onDestroy)
        {
            
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.deleteUIPrefab);
            gameObject.GetComponent<ConfirmUI>().onConfirm = onConfirm;
            gameObject.GetComponent<ConfirmUI>().onDestroy = onDestroy;
            gameObject.transform.Find("MainPanel/GameSummaryPanel").GetComponent<LevelSummaryPanel>().Init(srleName);
            return gameObject;
        }
        public void SetSelectedIdx(int idx)
        {
            this.selectedIdx = idx;
            try
            {
                this.settingToggleStates = true;
                this.levelToggles[idx].Select();

                this.summaryPanel.GetComponent<LevelSummaryPanel>().Init(availLevels[idx]);

                //this.playButton.interactable = !this.availGames[idx].isInvalid && !this.availGames[idx].gameOver && !this.autoSaveDirector.IsLoadingGame();
            }
            finally
            {
                this.settingToggleStates = false;
            }
        }
    }
}