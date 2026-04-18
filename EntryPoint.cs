using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using SRLE.Commands;
using SRLE.Components;
using SRLE.Components.MainMenuUIs;
using SRLE.Models;
using SRML;
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
        private static readonly Assembly ModAssembly = typeof(EntryPoint).Assembly;
        private static readonly List<Assembly> LibAssemblies = new List<Assembly>();
        public const string Version = "1.0.0";

        public EntryPoint()
        {
            foreach (var file in ModAssembly.GetManifestResourceNames())
                if (file.ToLower().EndsWith(".dll"))
                {
                    try
                    {
                        var stream = ModAssembly.GetManifestResourceStream(file);
                        var bytes = new byte[stream.Length];
                        _ = stream.Read(bytes, 0, bytes.Length);
                        LibAssemblies.Add(Assembly.Load(bytes));
                    }
                    catch (Exception e)
                    {
                        // ConsoleInstance.Log("An error occured loading a resource assembly: " + e);
                    }
                }
            AppDomain.CurrentDomain.AssemblyResolve += (x, y) =>
            {
                var name = new AssemblyName(y.Name).Name;
                return LibAssemblies.FirstOrDefault(lA => lA.GetName().Name == name);
            };
        }
        
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

            SRCallbacks.PreSaveGameLoad += context => SRLEConverter.ConvertToBuildObjects();

            GameObject manager = new GameObject("SRLE");
            manager.AddComponent<SRLEMod>();
            manager.hideFlags |= HideFlags.HideAndDontSave;

            SRCallbacks.PreSaveGameLoaded += OnSaveGamePreLoaded;
            SRCallbacks.OnMainMenuLoaded += SetupMainMenu;


        }

        public override void Load()
        {
            foreach (var id in Gadget.DECO_CLASS.Concat(Gadget.LAMP_CLASS))
            {
                if (SRSingleton<GameContext>.Instance.LookupDirector.gadgetDefinitionDict.TryGetValue(id, out var gadgetDefinition))
                {
                    if (gadgetDefinition && gadgetDefinition.prefab)
                    {
                        var copyPrefab = PrefabUtils.CopyPrefab(gadgetDefinition.prefab);
                        copyPrefab.RemoveComponent<Gadget>();
                        ObjectManager.RegisterObject("srle", "Mods", copyPrefab.name, copyPrefab);
                    }
                }
               
            }
        }

        // ── Save-game loaded ──────────────────────────────────────────────────

        private static void OnSaveGamePreLoaded(SceneContext context)
        {
            if (LevelManager.CurrentMode == LevelManager.Mode.NONE)
                return;
            
            ObjectManager.World = new GameObject("SRLEWorld");
            ObjectManager.World.hideFlags |= HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(ObjectManager.World);
            UIInitializer.Initialize();
            foreach (var id in SaveManager.CurrentLevel.BuildObjects.Keys)
            {
                foreach (var data in SaveManager.CurrentLevel.BuildObjects[id])
                {
                    ObjectManager.RequestObject(id, idClass =>
                    {
                        if (idClass != null)
                        {
                            GameObject obj = Object.Instantiate(idClass.gameObject, BuildObjectData.Vector3Save.RevertToVector3(data.Pos), Quaternion.Euler(BuildObjectData.Vector3Save.RevertToVector3(data.Rot)), ObjectManager.World.transform);
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

            ApplyWorldType(SaveManager.CurrentLevel.WorldType);
        }

        private static void ApplyWorldType(WorldType worldType)
        {
            switch (worldType)
            {
                case WorldType.STANDARD:
                    break;
                case WorldType.SEA:
                    foreach (var zoneDirector in Object.FindObjectsOfType<ZoneDirector>())
                    {
                        for (int i = 0; i < zoneDirector.transform.childCount; i++)
                        {
                            var transform = zoneDirector.transform.GetChild(i);
                            if (transform.name.Equals("Sea"))
                                continue;
                            transform.gameObject.SetActive(false);
                        }
                    }
                    break;
                case WorldType.VOID:
                    foreach (var zoneDirector in Object.FindObjectsOfType<ZoneDirector>())
                    {
                        for (int i = 0; i < zoneDirector.transform.childCount; i++)
                        {
                            zoneDirector.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                    break;
                case WorldType.DESERT:
                    foreach (var zoneDirector in Object.FindObjectsOfType<ZoneDirector>())
                    {
                        for (int i = 0; i < zoneDirector.transform.childCount; i++)
                        {
                            var transform = zoneDirector.transform.GetChild(i);
                            if (transform.name.Equals("SandSea"))
                            {
                                transform.gameObject.SetActive(true);
                                transform.position = new Vector3(89.4098f, -3f,-145.1977f);
                                continue;
                            }
                            transform.gameObject.SetActive(false);                        
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldType), worldType, null);
            }
        }

        // ── Main menu setup ───────────────────────────────────────────────────

        private static void SetupMainMenu(MainMenuUI menu)
        {
            // Destroy world objects left over from a previous session
            if (ObjectManager.World != null)
            {
                Object.Destroy(ObjectManager.World);
                ObjectManager.World = null;
            }
            ObjectManager.BuildObjects.Clear();
            SaveManager.CurrentLevel = null;
            LevelManager.SetMode(LevelManager.Mode.NONE);
            ChunkManager.Clear();
            RuntimeGizmo.UndoRedo.UndoRedoManager.Clear();

            SRLENewLevelUI.AllSprites = ZoneDirector.zonePediaIdLookup
                .Values
                .Distinct()
                .Select(z => z.GetIcon())
                .ToList();

            var button = MainMenuUtils.AddMainMenuButtonWithTranslation(menu, "SRLE", "b.srle", () => OpenSRLELobby(menu));
            button.name = "SRLEButton";
            button.transform.Find("Text").GetComponent<XlateText>().SetKey("b.srle");
            button.transform.SetSiblingIndex(4);
        }

        private static void OpenSRLELobby(MainMenuUI menu)
        {
            var lobbyObj = (menu.expoSelectGameUI).InstantiateInactive(true);
            lobbyObj.name = "SRLE_UI";
            Object.Destroy(lobbyObj.GetComponent<ExpoGameSelectUI>());
            var lobbyUI = lobbyObj.AddComponent<BaseUI>();
            lobbyUI.onDestroy = null;
            lobbyUI.onDestroy = () => menu?.gameObject.SetActive(true);
            menu.gameObject.SetActive(false);
            lobbyObj.gameObject.SetActive(true);

            var panel = lobbyObj.transform.Find("Panel");
            panel.transform.Find("Window Title").GetComponentInChildren<XlateText>().SetKey("l.srle.window_title");
            panel = panel.transform.Find("Panel");

            var createLevel = panel.GetChild(0);
            createLevel.GetComponentInChildren<XlateText>().SetKey("l.srle.create_a_level");
            (createLevel.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                () => OpenNewLevelUI(menu, lobbyObj, lobbyUI));

            var loadLevel = panel.GetChild(1);
            loadLevel.GetComponentInChildren<XlateText>().SetKey("l.srle.load_a_level");
            (loadLevel.GetComponentInChildren<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                () => OpenLoadLevelUI(menu, lobbyObj, lobbyUI));
        }

        private static void OpenNewLevelUI(MainMenuUI menu, GameObject lobbyObj, BaseUI lobbyUI)
        {
            var newGameUI = menu.newGameUI.InstantiateInactive();
            newGameUI.name = "SRLENewLevel";
            var gameUI = newGameUI.GetComponent<NewGameUI>();
            var srleNewLevelUI = newGameUI.AddComponent<SRLENewLevelUI>();
            srleNewLevelUI.onDestroy += () =>
            {
                if (lobbyUI)
                    lobbyUI.Close();
            };
            srleNewLevelUI.gameIconPrefab = gameUI.gameIconPrefab;
            var panel = newGameUI.transform.Find("Panel").gameObject;
            panel.FindChild("Title").GetComponent<XlateText>().SetKey("l.srle.create_a_level");

            GameObject infoPanel = panel.FindChild("InfoPanel");
            (panel.FindChild("BackButton", true).GetComponent<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                () =>
                {
                    lobbyUI.Close();
                    srleNewLevelUI.Close();
                });
            infoPanel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.level_name");
            srleNewLevelUI.levelNameField = gameUI.gameNameField;
            infoPanel.GetChild(3).GetComponent<XlateText>().SetKey("l.srle.choose_level_icon");
            infoPanel.GetChild(6).GetComponent<XlateText>().SetKey("l.srle.select_world_type");

            var modeOuterPanel = infoPanel.GetChild(7).GetChild(0);
            srleNewLevelUI.worldModeDescription = gameUI.gameModeText;
            srleNewLevelUI.iconGroup = gameUI.iconGroup;
            srleNewLevelUI.iconTabByMenuKeys = gameUI.iconTabByMenuKeys;

            srleNewLevelUI.rightIconButton = gameUI.rightIconButton;
            (srleNewLevelUI.rightIconButton.onClick = new Button.ButtonClickedEvent()).AddListener(srleNewLevelUI.SelectNextIcon);

            srleNewLevelUI.leftIconButton = gameUI.leftIconButton;
            (srleNewLevelUI.leftIconButton.onClick = new Button.ButtonClickedEvent()).AddListener(srleNewLevelUI.SelectPreviousIcon);

            var seaToggle = modeOuterPanel.GetChild(0);
            seaToggle.name = "SeaToggle";
            seaToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.sea");
            srleNewLevelUI.SeaToggle = seaToggle.GetComponent<SRToggle>();

            var desertToggle = modeOuterPanel.GetChild(1);
            desertToggle.name = "DesertToggle";
            desertToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.desert");
            srleNewLevelUI.DesertToggle = desertToggle.GetComponent<SRToggle>();

            var voidToggle = modeOuterPanel.GetChild(2);
            voidToggle.name = "VoidToggle";
            voidToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.void");
            srleNewLevelUI.VoidToggle = voidToggle.GetComponent<SRToggle>();

            var standardToggle = Object.Instantiate(modeOuterPanel.GetChild(0).gameObject, modeOuterPanel.transform);
            standardToggle.name = "StandardToggle";
            standardToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type.standard");
            srleNewLevelUI.StandardToggle = standardToggle.GetComponent<SRToggle>();
            standardToggle.transform.SetSiblingIndex(0);

            // var startButton = panel.FindChild("StartButton", true)?.GetComponent<Button>()
            //                ?? gameUI.GetComponentInChildren<Button>(true);
            // if (startButton != null)
            //     (startButton.onClick = new Button.ButtonClickedEvent()).AddListener(srleNewLevelUI.CreateNewLevel);

            Object.DestroyImmediate(gameUI);
            newGameUI.SetActive(true);
            lobbyObj.SetActive(false);
        }

        private static void OpenLoadLevelUI(MainMenuUI menu, GameObject lobbyObj, BaseUI lobbyUI)
        {
            var loadLevelUI = GameObjectUtils.InstantiateInactive(menu.loadGameUI);
            loadLevelUI.name = "SRLELoadLevelUI";

            var srleLoadLevelUI = loadLevelUI.AddComponent<SRLELoadLevelUI>();
            srleLoadLevelUI.onDestroy = () =>
            {
                if (lobbyUI)
                    lobbyUI.Close();
            };

            GameObject panel = loadLevelUI.GetChild(0).gameObject;
            panel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.load_a_level");
            (panel.transform.Find("CloseButton").GetComponent<Button>().onClick = new Button.ButtonClickedEvent()).AddListener(
                () =>
                {
                    lobbyUI.Close();
                    srleLoadLevelUI.Close();
                });

            var loadGameUI = loadLevelUI.GetComponent<LoadGameUI>();
            srleLoadLevelUI.loadingPanel = loadGameUI.loadingPanel;
            srleLoadLevelUI.noSavesPanel = loadGameUI.noSavesPanel;
            srleLoadLevelUI.deleteUIPrefab = loadGameUI.deleteUIPrefab.InstantiateInactive(true);
            srleLoadLevelUI.deleteUIPrefab.SetActive(true);

            var gameSummaryPanelDeleteUIObj = srleLoadLevelUI.deleteUIPrefab.transform.Find("MainPanel/GameSummaryPanel").gameObject;
            var levelSummaryPanelDeleteUI = gameSummaryPanelDeleteUIObj.AddComponent<LevelSummaryPanel>();
            var gameSummaryPanelDeleteUI = levelSummaryPanelDeleteUI.GetComponent<GameSummaryPanel>();
            levelSummaryPanelDeleteUI.objectsAmountText = gameSummaryPanelDeleteUI.dayText;
            levelSummaryPanelDeleteUI.levelIcon = gameSummaryPanelDeleteUI.gameIcon;
            levelSummaryPanelDeleteUI.modeText = gameSummaryPanelDeleteUI.modeText;
            levelSummaryPanelDeleteUI.levelNameText = gameSummaryPanelDeleteUI.gameNameText;
            levelSummaryPanelDeleteUI.modeDescText = gameSummaryPanelDeleteUI.modeDescText;
            levelSummaryPanelDeleteUI.fileSizeText = gameSummaryPanelDeleteUI.pediaText;
            Object.Destroy(gameSummaryPanelDeleteUI);

            srleLoadLevelUI.noSavesPanel.GetComponentInChildren<XlateText>().SetKey("m.srle.no_saved_levels");
            srleLoadLevelUI.summaryPanel = loadGameUI.summaryPanel.gameObject;
            srleLoadLevelUI.loadGameButtonPrefab = loadGameUI.loadGameButtonPrefab;
            srleLoadLevelUI.loadButtonPanel = loadGameUI.loadButtonPanel;
            srleLoadLevelUI.scroller = loadGameUI.scroller;

            var gameSummaryPanel = srleLoadLevelUI.summaryPanel.GetComponent<GameSummaryPanel>();
            var levelSummaryPanel = srleLoadLevelUI.summaryPanel.AddComponent<LevelSummaryPanel>();
            levelSummaryPanel.objectsAmountText = gameSummaryPanel.dayText;
            levelSummaryPanel.levelIcon = gameSummaryPanel.gameIcon;
            levelSummaryPanel.modeText = gameSummaryPanel.modeText;
            levelSummaryPanel.levelNameText = gameSummaryPanel.gameNameText;
            levelSummaryPanel.modeDescText = gameSummaryPanel.modeDescText;
            levelSummaryPanel.fileSizeText = gameSummaryPanel.pediaText;
            gameSummaryPanel.currencyText.transform.parent.gameObject.SetActive(false);
            levelSummaryPanel.gameObject.FindChild(gameSummaryPanel.versionText.gameObject.name, true).SetActive(false);
            Object.DestroyImmediate(gameSummaryPanel);

            Object.DestroyImmediate(loadGameUI);
            loadLevelUI.SetActive(true);
            lobbyObj.SetActive(false);
        }
    }
}
