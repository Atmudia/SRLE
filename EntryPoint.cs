using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using SRLE.Commands;
using SRLE.Components;
using SRLE.Components.MainMenuUIs;
using SRLE.Models;
using SRML;
using SRML.Console;
using SRML.SR;
using SRML.SR.UI.Utils;
using SRML.Utils;
using UnityEngine;
using UnityEngine.UI;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;

namespace SRLE
{
    public class EntryPoint : ModEntryPoint
    {
        public new static Console.ConsoleInstance ConsoleInstance = new Console.ConsoleInstance("SRLE");
        public const string Version = "1.0.0";
        private static string[] m_DefaultRemovedScripts = new string[]
        {
            "ActivateOnProgressRange",
            "DeactivateOnGameMode",
            "DeactivateOnDLCDisabled",
            "PuzzleTeleportLock"
        };
        
        public override void PreLoad()
        {
            foreach (var type in AccessTools.GetTypesFromAssembly(typeof(EntryPoint).Assembly))
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            HarmonyInstance.PatchAll();
            Console.RegisterCommand(new ConvertBetterBuildCommand());
            Console.RegisterCommand(new DisableOcclusionCommand());
            
            TranslationPatcher.AddUITranslation("l.srle.window_title", "SRLE - Slime Rancher Level Editor");
            TranslationPatcher.AddUITranslation("l.srle.load_a_level", "Load a Level");
            TranslationPatcher.AddUITranslation("l.srle.create_a_level", "Create a Level");
            TranslationPatcher.AddUITranslation("b.srle", "SRLE");
            
            TranslationPatcher.AddUITranslation("l.srle.level_name", "Level Name");
            TranslationPatcher.AddUITranslation("m.srle.default_level_name", "Level {0}");
            TranslationPatcher.AddUITranslation("l.srle.object_count", "Objects: {0}");
            TranslationPatcher.AddUITranslation("l.srle.filesize", "Size: {0}");
			

            TranslationPatcher.AddUITranslation("l.srle.choose_level_icon", "Choose a Level Icon");
            TranslationPatcher.AddUITranslation("l.srle.select_world_type", "Select a World Type");

            TranslationPatcher.AddUITranslation("l.srle.world_type.sea", "Slime Sea");
            TranslationPatcher.AddUITranslation("l.srle.world_type.desert", "Glass Desert Sea");
            TranslationPatcher.AddUITranslation("l.srle.world_type.void", "Complete Void");
            TranslationPatcher.AddUITranslation("l.srle.world_type.standard", "Standard World");

            TranslationPatcher.AddUITranslation("m.srle.desc.worldType.sea",
                "A blank world with the slime sea for you to build your own ranch.");
            TranslationPatcher.AddUITranslation("m.srle.desc.worldType.desert",
                "A blank world with the Glass Desert's slime sea for you to make your own desert.");
            TranslationPatcher.AddUITranslation("m.srle.desc.worldType.standard",
                "The normal slime rancher map, prebuilt with everything the normal map has.");
            TranslationPatcher.AddUITranslation("m.srle.desc.worldType.void",
                "A completely blank world, no slime sea, no sea bed, no objects, build whatever you want here.");

            TranslationPatcher.AddUITranslation("m.srle.no_saved_levels", "No Saved Levels Available");
            TranslationPatcher.AddUITranslation("m.srle.confirm_delete", "Are you sure you wish to permanently delete this level?");
            
            SRCallbacks.PreSaveGameLoad += context =>
            {
                SRLEConverter.ConvertToBuildObjects();
            };
            GameObject manager = new GameObject("SRLE");
            manager.AddComponent<SRLEMod>();
            manager.hideFlags |= HideFlags.HideAndDontSave;

            SRCallbacks.PreSaveGameLoaded += context =>
            {
                if (LevelManager.CurrentMode == LevelManager.Mode.NONE)
                    return;
                
                ObjectManager.CachedGameObjects = new GameObject(nameof(ObjectManager.CachedGameObjects))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                ObjectManager.World = new GameObject("SRLEWorld");
                ObjectManager.World.hideFlags |= HideFlags.HideAndDontSave;
                Object.DontDestroyOnLoad(ObjectManager.World);
                UIInitializer.Initialize();

                var idClasses = ObjectManager.BuildObjectsData;
                foreach (var idClass in idClasses)
                {
                    var gameObjectPath = GameObject.Find(idClass.Value.Path);
                    if (!gameObjectPath)
                    {
                        ConsoleInstance.Log($"Object with ID {idClass.Value.Id} could not be found");
                        continue;
                    }
                    var beforeState = gameObjectPath.activeInHierarchy;
                    gameObjectPath.SetActive(false);
                    var buildObject = Object.Instantiate(gameObjectPath, new Vector3(0,0,0), gameObjectPath.transform.rotation, ObjectManager.CachedGameObjects.transform);
                    buildObject.name = $"{idClass.Value.Id} {gameObjectPath.name}";
                    foreach (MonoBehaviour script in buildObject.GetComponentsInChildren<MonoBehaviour>(true))
                    {
                        foreach (var defaultRemove in m_DefaultRemovedScripts)
                        {
                            if (script.GetType().ToString().Equals(defaultRemove, StringComparison.CurrentCultureIgnoreCase))
                            {
                                UnityEngine.Object.Destroy(script);
                            }
                        }
                    }

                    TeleportDestination destination = buildObject.GetComponentInChildren<TeleportDestination>(true);
                    TeleportSource source = buildObject.GetComponentInChildren<TeleportSource>(true);
                    if (destination != null)
                    {
                        destination.teleportDestinationName = "NotSet";
                    }
                    if (source != null)
                    {
                        source.activated = false;
                        source.activationBlocker = null;
                        source.waitForExternalActivation = false;
                        source.activationProgress = ProgressDirector.ProgressType.NONE;
                        source.blockingGenerator = null;
                        source.destinationSetName = "NotSet";
                    }

                    JournalEntry journal = buildObject.GetComponentInChildren<JournalEntry>();
                    if (journal != null)
                    {
                        journal.ensureProgress = Array.Empty<ProgressDirector.ProgressType>();
                    }
                    gameObjectPath.SetActive(beforeState);
                    idClass.Value.GameObject = buildObject;
                }
                foreach (var id in SaveManager.CurrentLevel.BuildObjects.Keys)
                {
                    EntryPoint.ConsoleInstance.Log(id);
                    foreach (var data in SaveManager.CurrentLevel.BuildObjects[id])
                    {
                        ObjectManager.RequestObject(id, idClass =>
                        {
                            if (idClass != null)
                            {
                                GameObject obj = Object.Instantiate(idClass.GameObject, BuildObjectData.Vector3Save.RevertToVector3(data.Pos), Quaternion.Euler(BuildObjectData.Vector3Save.RevertToVector3(data.Rot)), ObjectManager.World.transform);
                                var buildObject = obj.AddComponent<BuildObject>();
                                buildObject.ID = idClass;
                                buildObject.SetData(data.Properties);
                                obj.transform.localScale = BuildObjectData.Vector3Save.RevertToVector3(data.Scale);
                                obj.SetActive(true);
                                ObjectManager.AddObject(id, obj);

                            }
                        });
                        
                        ToolbarUI.Instance.UpdateStatus();
                    }
                }
                ToolbarUI.Instance.UpdateStatus();


            };
            SRCallbacks.OnMainMenuLoaded += menu =>
            {
                SRLENewLevelUI.AllSprites = ZoneDirector.zonePediaIdLookup
                    .Values
                    .Distinct()
                    .Select(z => z.GetIcon())
                    .ToList();
                var addMainMenuButton = MainMenuUtils.AddMainMenuButtonWithTranslation(menu, "SRLE", "b.srle", () =>
                {
                    var instantiateAndWaitForDestroy = (menu.expoSelectGameUI).InstantiateInactive(true);
                    instantiateAndWaitForDestroy.name = "SRLE_UI";
                    Object.Destroy(instantiateAndWaitForDestroy.GetComponent<ExpoGameSelectUI>());
                    var expoGameSelectUI = instantiateAndWaitForDestroy.AddComponent<BaseUI>();
                    expoGameSelectUI.onDestroy = null;
                    expoGameSelectUI.onDestroy = () =>
                    { 
                        menu?.gameObject.SetActive(true);
                    };
                    menu.gameObject.SetActive(false);
                    instantiateAndWaitForDestroy.gameObject.SetActive(true);
                    
                    var panel = instantiateAndWaitForDestroy.transform.Find("Panel");
                    panel.transform.Find("Window Title").GetComponentInChildren<XlateText>().SetKey("l.srle.window_title");
                    panel = panel.transform.Find("Panel");
                    var createLevel = panel.GetChild(0);
                    createLevel.GetComponentInChildren<XlateText>().SetKey("l.srle.create_a_level");
                    (createLevel.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(() =>
                    {
                        var newGameUI = menu.newGameUI.InstantiateInactive();
                        newGameUI.name = "SRLENewLevel";
                        var gameUI = newGameUI.GetComponent<NewGameUI>();
                        var srleNewLevelUI = newGameUI.AddComponent<SRLENewLevelUI>();
                        srleNewLevelUI.onDestroy += () =>
                        {
                            if (expoGameSelectUI)
                                expoGameSelectUI.Close();
                        };
                        srleNewLevelUI.gameIconPrefab = gameUI.gameIconPrefab;
                        var panel = newGameUI.transform.Find("Panel").gameObject;
                        panel.FindChild("Title").GetComponent<XlateText>().SetKey("l.srle.create_a_level");

                        GameObject infoPanel = panel.FindChild("InfoPanel");
                        (panel.FindChild("BackButton", true).GetComponent<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                            () => {
                                    expoGameSelectUI.Close();
		                            srleNewLevelUI.Close();
	                            });
                        infoPanel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.level_name");
                        srleNewLevelUI.levelNameField = gameUI.gameNameField;
                        infoPanel.GetChild(3).GetComponent<XlateText>().SetKey("l.srle.choose_level_icon");
                        infoPanel.GetChild(6).GetComponent<XlateText>().SetKey("l.srle.select_world_type");

                        var ModeOuterPanel = infoPanel.GetChild(7).GetChild(0);
                        srleNewLevelUI.worldModeDescription = gameUI.gameModeText;
                        srleNewLevelUI.iconGroup = gameUI.iconGroup;
                        srleNewLevelUI.iconTabByMenuKeys = gameUI.iconTabByMenuKeys;

                        srleNewLevelUI.rightIconButton = gameUI.rightIconButton;
                        (srleNewLevelUI.rightIconButton.onClick = new Button.ButtonClickedEvent()).AddListener(srleNewLevelUI.SelectNextIcon);
                            
                        srleNewLevelUI.leftIconButton = gameUI.leftIconButton;
                        (srleNewLevelUI.leftIconButton.onClick = new Button.ButtonClickedEvent()).AddListener(srleNewLevelUI.SelectPreviousIcon);
                            



                        var SeaToggle = ModeOuterPanel.GetChild(0);
                        SeaToggle.name = "SeaToggle";
                        SeaToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.sea");
                        srleNewLevelUI.SeaToggle = SeaToggle.GetComponent<SRToggle>();

                        var DesertToggle = ModeOuterPanel.GetChild(1);
                        DesertToggle.name = "DesertToggle";
                        DesertToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.desert");
                        srleNewLevelUI.DesertToggle = DesertToggle.GetComponent<SRToggle>();

                            
                        var VoidToggle = ModeOuterPanel.GetChild(2);
                        VoidToggle.name = "VoidToggle";
                        VoidToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.void");
                        srleNewLevelUI.VoidToggle = VoidToggle.GetComponent<SRToggle>();

                            
                            
                        var StandardToggle = Object.Instantiate(ModeOuterPanel.GetChild(0).gameObject, ModeOuterPanel.transform);
                        StandardToggle.name = "StandardToggle";
                        StandardToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.standard");
                        srleNewLevelUI.StandardToggle = StandardToggle.GetComponent<SRToggle>();
                        StandardToggle.transform.SetSiblingIndex(0);
                        Object.DestroyImmediate(gameUI);
                        newGameUI.SetActive(true);
                        instantiateAndWaitForDestroy.SetActive(false);

                    });
                    var loadLevel = panel.GetChild(1);
                    loadLevel.GetComponentInChildren<XlateText>().SetKey("l.srle.load_a_level");
                    (loadLevel.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                        () =>
                        {
                            var loadLevelUI = GameObjectUtils.InstantiateInactive(menu.loadGameUI);
                            loadLevelUI.name = "SRLELoadLevelUI";

                            var srleNewLevelUI = loadLevelUI.AddComponent<SRLE.Components.SRLELoadLevelUI>();
                            srleNewLevelUI.onDestroy = () =>
                            { 
                                if (expoGameSelectUI)
                                    expoGameSelectUI.Close();

                            };
                            GameObject panel = loadLevelUI.GetChild(0).gameObject;
                            panel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.load_a_level");
                            (panel.transform.Find("CloseButton").GetComponent<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                                () =>
                                {
                                    expoGameSelectUI.Close();
                                    srleNewLevelUI.Close();
                                    
                                });
                            var loadGameUI = loadLevelUI.GetComponent<LoadGameUI>();
                            srleNewLevelUI.loadingPanel = loadGameUI.loadingPanel;
                            srleNewLevelUI.noSavesPanel = loadGameUI.noSavesPanel;
                            srleNewLevelUI.deleteUIPrefab = loadGameUI.deleteUIPrefab.InstantiateInactive(true);
                            srleNewLevelUI.deleteUIPrefab.SetActive(true);
                            var gameSummaryPanelDeleteUIObj = srleNewLevelUI.deleteUIPrefab.transform.Find("MainPanel/GameSummaryPanel").gameObject;
                            var levelSummaryPanelDeleteUI = gameSummaryPanelDeleteUIObj.AddComponent<LevelSummaryPanel>();
                            var gameSummaryPanelDeleteUI = levelSummaryPanelDeleteUI.GetComponent<GameSummaryPanel>();
                            levelSummaryPanelDeleteUI.objectsAmountText = gameSummaryPanelDeleteUI.dayText;
                            levelSummaryPanelDeleteUI.levelIcon = gameSummaryPanelDeleteUI.gameIcon;
                            levelSummaryPanelDeleteUI.modeText = gameSummaryPanelDeleteUI.modeText;
                            levelSummaryPanelDeleteUI.levelNameText = gameSummaryPanelDeleteUI.gameNameText;
                            levelSummaryPanelDeleteUI.modeDescText = gameSummaryPanelDeleteUI.modeDescText;
                            levelSummaryPanelDeleteUI.fileSizeText = gameSummaryPanelDeleteUI.pediaText;
                            Object.Destroy(gameSummaryPanelDeleteUI);
                            
                            
                            srleNewLevelUI.noSavesPanel.GetComponentInChildren<XlateText>().SetKey("m.srle.no_saved_levels");
                            srleNewLevelUI.summaryPanel = loadGameUI.summaryPanel.gameObject;
                            srleNewLevelUI.loadGameButtonPrefab = loadGameUI.loadGameButtonPrefab;
                            srleNewLevelUI.loadButtonPanel = loadGameUI.loadButtonPanel;
                            srleNewLevelUI.scroller = loadGameUI.scroller;
                            var gameSummaryPanel = srleNewLevelUI.summaryPanel.GetComponent<GameSummaryPanel>();
                            var levelSummaryPanel = srleNewLevelUI.summaryPanel.AddComponent<LevelSummaryPanel>();
                            levelSummaryPanel.objectsAmountText = gameSummaryPanel.dayText;
                            levelSummaryPanel.levelIcon = gameSummaryPanel.gameIcon;
                            levelSummaryPanel.modeText = gameSummaryPanel.modeText;
                            levelSummaryPanel.levelNameText = gameSummaryPanel.gameNameText;
                            levelSummaryPanel.modeDescText = gameSummaryPanel.modeDescText;
                            levelSummaryPanel.fileSizeText = gameSummaryPanel.pediaText;
                            gameSummaryPanel.currencyText.transform.parent.gameObject.SetActive(false);
                            levelSummaryPanel.gameObject.FindChild(gameSummaryPanel.versionText.gameObject.name, true).SetActive(false);
                            Object.DestroyImmediate(gameSummaryPanel);


                            // levelSummaryPanel.transform.Find("MainPanel/ValidPanel/InfoPanel/Panel").gameObject.SetActive(false);
                            
                            Object.DestroyImmediate(loadGameUI);
                            loadLevelUI.SetActive(true);
                            instantiateAndWaitForDestroy.SetActive(false);
                            
                        });
                    
                                
                });
                addMainMenuButton.name = "SRLEButton";
                addMainMenuButton.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
                addMainMenuButton.transform.SetSiblingIndex(4);
               
            };

        }

        public override void Load()
        {
            
        }
        
        
        
    //     public class TestHashCodeCommand : ConsoleCommand
    //     {
    //         public override bool Execute(string[] args)
    //         {
    //             Ray ray = SRLECamera.Instance.camera.ScreenPointToRay(Input.mousePosition);
    //             if (!Physics.Raycast(ray, out var hit))
    //                 return false;
    //             GetObjectByHashCode(hit.collider.gameObject);
    //             return true;
    //         }
    //
    //         public override string ID => "testhashcode";
    //         public override string Usage => ID;
    //         public override string Description => "Test hashcode";
    //     }
    // }
    }
}