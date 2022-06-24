using SRLE.Components;
using SRML.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace SRLE
{
    public static class SRLEUIMenu
    {
        public static bool playing = false;
        public static void SetAllButtonsEXPO(GameObject expoUI, MainMenuUI menu)
        {
            
            var panel = expoUI.transform.Find("Panel");
            panel.transform.Find("Window Title").GetComponentInChildren<XlateText>().SetKey("l.srle.window_title");
            panel = panel.transform.Find("Panel");
            panel.GetChild(0).gameObject.SetActive(false);
            var loadALevel = panel.GetChild(1);
            
            
            loadALevel.GetComponentInChildren<XlateText>().SetKey("l.srle.create_a_level");
            var button = loadALevel.GetComponent<Button>();
            
            button.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                var newGameUI = GameObjectUtils.InstantiateInactive(menu.newGameUI);
                newGameUI.name = "SRLENewLevel";


                var gameUI = newGameUI.GetComponent<NewGameUI>();
                var srleNewLevelUI = newGameUI.AddComponent<SRLE.Components.SRLENewLevelUI>();
                srleNewLevelUI.gameIconPrefab = gameUI.gameIconPrefab;
                /*newGameUI.GetComponentsInChildren<Button>().Select(x =>
                {
                    /*if (x.enabled)
                        
                        return false;
                        */
                //});

                
            
                
                var panel = newGameUI.transform.Find("Panel").gameObject;
                
                
                panel.FindChild("Title").GetComponent<XlateText>().SetKey("l.srle.create_a_level");
                panel.GetChild(2).GetChild(1).GetComponent<Button>().RemoveAllListeners().onClick.AddListener(() => srleNewLevelUI.CreateNewLevel(panel.GetChild(2).GetChild(0).GetComponent<Button>()));

                GameObject infoPanel = panel.FindChild("InfoPanel");
                panel.FindChild("BackButton", true).GetComponent<Button>().RemoveAllListeners().onClick.AddListener(() => srleNewLevelUI.Close());
            

                infoPanel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.level_name");
                srleNewLevelUI.levelNameField = gameUI.gameNameField;
                infoPanel.GetChild(3).GetComponent<XlateText>().SetKey("l.srle.choose_level_icon");
                infoPanel.GetChild(6).GetComponent<XlateText>().SetKey("l.srle.select_world_type");

                var ModeOuterPanel = infoPanel.GetChild(7).GetChild(0);
                srleNewLevelUI.worldModeDescription = gameUI.gameModeText;
                srleNewLevelUI.iconGroup = gameUI.iconGroup;
                srleNewLevelUI.iconTabByMenuKeys = gameUI.iconTabByMenuKeys;

                srleNewLevelUI.rightIconButton = gameUI.rightIconButton;
                srleNewLevelUI.rightIconButton.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.SelectNextIcon);
                
                srleNewLevelUI.leftIconButton = gameUI.leftIconButton;
                srleNewLevelUI.leftIconButton.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.SelectPrevIcon);
                



                var SeaToggle = ModeOuterPanel.GetChild(0);
                SeaToggle.name = "SeaToggle";
                SeaToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type_sea");
                srleNewLevelUI.SeaToggle = SeaToggle.GetComponent<SRToggle>();

                var DesertToggle = ModeOuterPanel.GetChild(1);
                DesertToggle.name = "DesertToggle";
                DesertToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type_desert");
                srleNewLevelUI.DesertToggle = DesertToggle.GetComponent<SRToggle>();

                
                var VoidToggle = ModeOuterPanel.GetChild(2);
                VoidToggle.name = "VoidToggle";
                VoidToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type_void");
                srleNewLevelUI.VoidToggle = VoidToggle.GetComponent<SRToggle>();

                
                
                var StandardToggle = Object.Instantiate(ModeOuterPanel.GetChild(0).gameObject, ModeOuterPanel.transform);
                StandardToggle.name = "StandardToggle";
                StandardToggle.GetComponentInChildren<XlateText>().SetKey("l.srle.world_type_standard");
                srleNewLevelUI.StandardToggle = StandardToggle.GetComponent<SRToggle>();
                StandardToggle.transform.SetSiblingIndex(0);



                srleNewLevelUI.onDestroy = () =>
                {
                    if (!playing)
                        menu.gameObject.SetActive(true);
                };

                
                expoUI.GetComponent<ExpoGameSelectUI>().Close();
                menu.gameObject.SetActive(false);
                Object.DestroyImmediate(gameUI);
                newGameUI.SetActive(true);

            });

            var createALevel = panel.GetChild(2);
            createALevel.GetComponentInChildren<XlateText>().SetKey("l.srle.load_a_level");
            button = createALevel.GetComponent<Button>();
            button.RemoveAllListeners().onClick.AddListener(() =>
            {
                
                var loadLevelUI = GameObjectUtils.InstantiateInactive(menu.loadGameUI);
                loadLevelUI.name = "SRLELoadLevelUI";

                var srleNewLevelUI = loadLevelUI.AddComponent<SRLE.Components.SRLELoadLevelUI>();
                GameObject panel = loadLevelUI.GetChild(0).gameObject;
                panel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.load_a_level");
                panel.transform.Find("CloseButton").GetComponent<Button>().RemoveAllListeners().onClick.AddListener(
                    () =>
                    {
                        srleNewLevelUI.Close();
                    });
                
                srleNewLevelUI.onDestroy = () =>
                {
                    if (!playing)
                        menu.gameObject.SetActive(true);
                };
                var loadGameUI = loadLevelUI.GetComponent<LoadGameUI>();
                srleNewLevelUI.loadingPanel = loadGameUI.loadingPanel;
                srleNewLevelUI.noSavesPanel = loadGameUI.noSavesPanel;
                
                srleNewLevelUI.noSavesPanel.GetComponentInChildren<XlateText>().SetKey("m.srle.no_saved_levels");
                srleNewLevelUI.summaryPanel = loadGameUI.summaryPanel.gameObject;
                var gameSummaryPanel = srleNewLevelUI.summaryPanel.GetComponent<GameSummaryPanel>();
                var levelSummaryPanel = srleNewLevelUI.summaryPanel.AddComponent<LevelSummaryPanel>();
                levelSummaryPanel.gameObject.FindChild(gameSummaryPanel.versionText.gameObject.name, true).SetActive(false);


                levelSummaryPanel.transform.Find("MainPanel/ValidPanel/InfoPanel/Panel").gameObject.SetActive(false);





                

                levelSummaryPanel.objectsAmountText = gameSummaryPanel.dayText;
                levelSummaryPanel.levelIcon = gameSummaryPanel.gameIcon;
                levelSummaryPanel.modeText = gameSummaryPanel.modeText;
                levelSummaryPanel.levelNameText = gameSummaryPanel.gameNameText;
                levelSummaryPanel.modeDescText = gameSummaryPanel.modeDescText;
                levelSummaryPanel.fileSizeText = gameSummaryPanel.pediaText;

                foreach (var button in levelSummaryPanel.GetComponentsInChildren<Button>())
                {
                    if (button.name == "PlayButton")
                    {
                        button.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.PlaySelectedLevel);
                    }
                    else
                    {
                        button.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.DeleteSelectedLevel);
                    }
                }
                
                //levelSummaryPanel.gameObject.GetChild(0).GetChild(3).GetChild(1).GetComponent<Button>().RemoveAllListeners().onClick.AddListener(srleNewLevelUI.DeleteSelectedLevel);


                
                Object.DestroyImmediate(gameSummaryPanel);
                
                
                
                srleNewLevelUI.loadButtonPanel = loadGameUI.loadButtonPanel;
                srleNewLevelUI.loadGameButtonPrefab = loadGameUI.loadGameButtonPrefab;
                srleNewLevelUI.deleteUIPrefab = loadGameUI.deleteUIPrefab;
                srleNewLevelUI.scroller = loadGameUI.scroller;
                srleNewLevelUI.deleteUIPrefab.FindChild("Message", true).GetComponent<XlateText>().SetKey("m.srle.confirm_delete");
                levelSummaryPanel = srleNewLevelUI.deleteUIPrefab.FindChild("GameSummaryPanel", true).AddComponent<LevelSummaryPanel>();
                var gameSummaryPanelDuplicate = srleNewLevelUI.deleteUIPrefab.GetComponentInChildren<GameSummaryPanel>();
                levelSummaryPanel.levelIcon = gameSummaryPanelDuplicate.gameIcon;
                levelSummaryPanel.objectsAmountText = gameSummaryPanelDuplicate.dayText;
                levelSummaryPanel.modeText = gameSummaryPanelDuplicate.modeText;
                levelSummaryPanel.levelNameText = gameSummaryPanelDuplicate.gameNameText;
                levelSummaryPanel.modeDescText = gameSummaryPanelDuplicate.modeDescText;
                levelSummaryPanel.fileSizeText = gameSummaryPanelDuplicate.pediaText;
                
                levelSummaryPanel.gameObject.FindChild(gameSummaryPanelDuplicate.versionText.gameObject.name, true).SetActive(false);


                levelSummaryPanel.transform.Find("MainPanel/ValidPanel/InfoPanel/Panel").gameObject.SetActive(false);

                
                    

                expoUI.GetComponent<ExpoGameSelectUI>().Close();
                
                




                Object.DestroyImmediate(loadGameUI);
                
                menu.gameObject.SetActive(false);
                loadLevelUI.gameObject.SetActive(true);

            });






        }
        
    }
}