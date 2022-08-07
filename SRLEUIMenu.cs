using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using MonomiPark.SlimeRancher.Persist;
//using MonomiPark.SlimeRancher.Persist;
using SRLE.Components;
using SRLE.SaveSystem;
using SRML;
using SRML.Console;
using SRML.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SRLE
{
    public static class SRLEUIMenu
    {
	    public static bool oldReturnToMenu;

	    public static bool returnToMenu
	    {
		    get
		    {
			    EntryPoint.SRLEConsoleInstance.Log($"Getter:{oldReturnToMenu}" );
			    return oldReturnToMenu;
		    }
		    set
		    {
			    EntryPoint.SRLEConsoleInstance.Log($"Setter:{value}" );
			    oldReturnToMenu = value;
		    }
	    }

	    public static void SetAllButtonsEXPO(GameObject expoUI, MainMenuUI menu)
        {
            
            var panel = expoUI.transform.Find("Panel");
            panel.transform.Find("Window Title").GetComponentInChildren<XlateText>().SetKey("l.srle.window_title");
            panel = panel.transform.Find("Panel");
            var loadALevel = panel.GetChild(1);
            
            
            loadALevel.GetComponentInChildren<XlateText>().SetKey("l.srle.create_a_level");
            var button = loadALevel.GetComponent<Button>();
            
            button.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
	            returnToMenu = false;

	            EntryPoint.SRLEConsoleInstance.Log("SRLENewLevel");
                var newGameUI = GameObjectUtils.InstantiateInactive(menu.newGameUI);
                newGameUI.name = "SRLENewLevel";


                var gameUI = newGameUI.GetComponent<NewGameUI>();
                var srleNewLevelUI = newGameUI.AddComponent<SRLE.Components.SRLENewLevelUI>();
                srleNewLevelUI.onDestroy = () =>
                { 
	                if (SRLEUIMenu.returnToMenu)
		                menu?.gameObject.SetActive(true);
	                SRLEUIMenu.returnToMenu = true;
                };
                srleNewLevelUI.gameIconPrefab = gameUI.gameIconPrefab;




                var panel = newGameUI.transform.Find("Panel").gameObject;
                
                
                panel.FindChild("Title").GetComponent<XlateText>().SetKey("l.srle.create_a_level");
                panel.GetChild(2).GetChild(1).GetComponent<Button>().RemoveAllListeners().onClick.AddListener(() => srleNewLevelUI.CreateNewLevel(panel.GetChild(2).GetChild(0).GetComponent<Button>()));

                GameObject infoPanel = panel.FindChild("InfoPanel");
                panel.FindChild("BackButton", true).GetComponent<Button>().RemoveAllListeners().onClick.AddListener(
	                () =>
	                {
		                returnToMenu = true;
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
                srleNewLevelUI.rightIconButton.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.SelectNextIcon);
                
                srleNewLevelUI.leftIconButton = gameUI.leftIconButton;
                srleNewLevelUI.leftIconButton.RemoveAllListeners().onClick.AddListener(srleNewLevelUI.SelectPrevIcon);
                



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
	            EntryPoint.SRLEConsoleInstance.Log("SRLELoadLevelUI");
	            returnToMenu = false;
	            expoUI.GetComponent<ExpoGameSelectUI>().Close();
	            var loadLevelUI = GameObjectUtils.InstantiateInactive(menu.loadGameUI);
                loadLevelUI.name = "SRLELoadLevelUI";

                var srleNewLevelUI = loadLevelUI.AddComponent<SRLE.Components.SRLELoadLevelUI>();
                srleNewLevelUI.onDestroy = () =>
                { 
	                if (SRLEUIMenu.returnToMenu)
		                menu?.gameObject.SetActive(true);

                };
                GameObject panel = loadLevelUI.GetChild(0).gameObject;
                panel.GetChild(0).GetComponent<XlateText>().SetKey("l.srle.load_a_level");
                panel.transform.Find("CloseButton").GetComponent<Button>().RemoveAllListeners().onClick.AddListener(
                    () =>
                    {
                        srleNewLevelUI.Close();
                    });
                
               
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

                
                    

                
                




                Object.DestroyImmediate(loadGameUI);
                menu.gameObject.SetActive(false);
                loadLevelUI.gameObject.SetActive(true);


            });
            
            
            var srleConverter = panel.GetChild(0).gameObject;
            srleConverter.transform.SetSiblingIndex(3);
            srleConverter.GetComponentInChildren<XlateText>().SetKey("l.srle.srle_converter");
            var buttonFromBB = srleConverter.GetComponent<Button>();
            
            
            if (SRModLoader.IsModPresent("betterbuild"))
            {
	            var parentFullName = new DirectoryInfo(SRML.FileSystem.LibPath).Parent.Parent.FullName;
	            var fileInfos = new DirectoryInfo(Path.Combine(parentFullName, "BetterBuild")).GetFiles();
                if (fileInfos.Length == 0 || fileInfos.FirstOrDefault(x => x.Extension == ".world") == null)
                {
                    buttonFromBB.interactable = false;
                    buttonFromBB.gameObject.GetChild(1).GetComponentInChildren<XlateText>().SetKey("l.srle.betterbuild.notfoundanysave");
                    return;

                }
                

                buttonFromBB.RemoveAllListeners().onClick.AddListener(() =>
                {
	                returnToMenu = false;
	                expoUI.GetComponent<ExpoGameSelectUI>().Close();
	                var idEntry = SRSingleton<SceneContext>.Instance.PediaDirector.entries.First(x => x.id == PediaDirector.Id.GADGETMODE).icon;

                    
                    List<PurchaseUI.Purchasable> purchaseUis = new List<PurchaseUI.Purchasable>();
                    GameObject purchaseUI = null;
                    foreach (var VARIABLE in fileInfos)             
                    {
                        if (VARIABLE.Extension == ".world")
                        {
                            var s = VARIABLE.Name.Replace(".world", string.Empty);
                            purchaseUis.Add(new PurchaseUI.Purchasable(s, idEntry, idEntry, s, 1, null, () =>
                            {
	                            if (File.Exists(Path.Combine(SRLEManager.Worlds.FullName, VARIABLE.Name.Replace(".world", ".srle"))))
	                            {
		                            SRSingleton<GameContext>.Instance.UITemplates.CreateConfirmDialog(
			                            "m.srle.replaceoldworld",
			                            () =>
			                            {
				                            SceneManager.LoadScene("worldGenerated");
				                            SRCallbacksUtils.AddSRCallbacksAndDeleteAfterLoading(context => ConvertBetterBuildToSRLE(VARIABLE, purchaseUI));
				                            
				                            SceneContext.onSceneLoaded += ctx =>
				                            {

				                            };

			                            });
		                           
		                            return;
	                            }

	                            SceneManager.LoadScene("worldGenerated");
	                            SRCallbacksUtils.AddSRCallbacksAndDeleteAfterLoading(context => ConvertBetterBuildToSRLE(VARIABLE, purchaseUI));
	                            


                            }, () => true, () => true));
                        }
                    }
	                purchaseUI = SRSingleton<GameContext>.Instance.UITemplates.CreatePurchaseUI(idEntry, "t.srle.srle_converter", purchaseUis.ToArray(), true,
		                () =>
		                { 
			                if (SRLEUIMenu.returnToMenu)
				                if (menu.gameObject is not null)
					                menu.gameObject.SetActive(true);
			                SRLEUIMenu.returnToMenu = true;

		                });
                    
                });
            }
            else
            {
                buttonFromBB.interactable = false;
                
            }
        }
        
        private static void ConvertBetterBuildToSRLE(FileInfo pathOfFile, GameObject purchaseUI)
        {
	        
	        EntryPoint.SRLEConsoleInstance.Log("SceneContext.onSceneLoaded");
            var invoke = AccessTools.TypeByName("PmkWqSqDqhyzqncfhFgkeiIeqAFfA").GetConstructor(System.Type.EmptyTypes)?.Invoke(System.Array.Empty<object>());
				if (invoke != null)
				{
					invoke.GetType().GetMethod("Load", new[]
						{
							typeof(Stream)
						})
						?.Invoke(invoke, new object[]
						{
							new FileStream(
								pathOfFile.FullName,
								FileMode.Open),
						});

					var type = invoke.GetType();
					var name = (string) type.GetField("NxbuhdGOoXjvYKGyvWOfJIkcIILn").GetValue(invoke);
					var value = type.GetField("PwuTlNLrWfEhoBKNfoDdbApcvSLW").GetValue(invoke);
					var type1 = value as System.Collections.IDictionary;


					var o = AccessTools.TypeByName("TMaFbmDBkLulRlsTCbClfQhfWwmCA").GetProperty("oMvEVShoQppTDOlWISWTPYakJbFL")?.GetValue(null) as IDictionary;
					List<object> objectsList1 = new List<object>();
					List<object> objectsList2 =new List<object>();
					foreach (var VARIABLE in type1.Keys)
					{
						objectsList1.Add(VARIABLE);
					}
					foreach (var VARIABLE in type1.Values)
					{
						objectsList2.Add(VARIABLE);
					}
					

					Dictionary<string, List<SRLESave>> objects = new Dictionary<string, List<SRLESave>>();

					var srleName = SRLEName.Create(pathOfFile.Name.Replace(".world", string.Empty), WorldType.STANDARD);


					var numbersAndWords = objectsList1.Zip(objectsList2, (n, w) => new {Key = n, Value = w});
					foreach (var nw in numbersAndWords)
					{
						EntryPoint.SRLEConsoleInstance.Log(nw.Key.ToString());
						if (!o.Contains(nw.Key)) continue;
						var o1 = o[nw.Key];
						var renderId = (int) o[nw.Key].GetType().GetField("RenderID").GetValue(o1);
						var path = (string) o[nw.Key].GetType().GetField("Path").GetValue(o1);
						var nameOfBB = (string) o[nw.Key].GetType().GetField("Name").GetValue(o1);
						var id = (uint) o[nw.Key].GetType().GetField("ID").GetValue(o1);


						foreach (var VARIABLE in (IList) nw.Value)
						{
							var type2 = VARIABLE.GetType();
							var value1 = type2.GetField("vnFsBQTGupjuJrOIzDrdnTGobnzCA").GetValue(VARIABLE);
							var position = value1.GetType().GetField("value").GetValue(value1);

							var value2 = type2.GetField("JGTZUTDwWNbSmlKDfhazTgObuUFf").GetValue(VARIABLE);
							var rotation = value2.GetType().GetField("value").GetValue(value2);

							var value3 = type2.GetField("oLhvsYqBzOpNVnXFhvqYIEWDabkJA").GetValue(VARIABLE);
							var scale = value3.GetType().GetField("value").GetValue(value3);

								
							var objectByHashCode = SRLEManager.GetObjectByHashCode(nameOfBB, path, renderId );

							if (nameOfBB == "Water 1")
							{
								objectByHashCode = "62";
							}

							var dictionary = type2.GetField("nWpPVSldMKqITEYvahfGlTmpFdlH").GetValue(VARIABLE) as IDictionary;

							List<object> objectsList3 = new List<object>();
							List<object> objectsList4 =new List<object>();
							foreach (var VARIABLE1 in dictionary.Keys)
							{
								objectsList3.Add(VARIABLE1);
							}
							foreach (var VARIABLE1 in dictionary.Values)
							{
								objectsList4.Add(VARIABLE1);
							}
								
							var keyValue = objectsList3.Zip(objectsList4, (n, w) => new {Key = n, Value = w});
		

								
								
							if (string.IsNullOrEmpty(objectByHashCode)) continue;
							SRLESave srleSaveToAdd = new SRLESave();
							(srleSaveToAdd.position = new Vector3V02()).value = (Vector3) position;
							(srleSaveToAdd.rotation = new Vector3V02()).value = (Vector3) rotation;
							(srleSaveToAdd.scale = new Vector3V02()).value = (Vector3) scale;
								
							foreach (var VARIABLE1 in keyValue)
							{
									
								var o2 = VARIABLE1.Value.GetType().GetField("IywkVfFOJnkxgsjKznhRmgZTQrlm").GetValue(VARIABLE1.Value);

								if (VARIABLE1.Key.ToString() == "journaltext")
								{
									(srleSaveToAdd.dictionaryWithProperties["JournalText"] = new SRLEProperty()).property = o2.ToString();
								}
								if (VARIABLE1.Key.ToString() == "tpdestination")
								{
									(srleSaveToAdd.dictionaryWithProperties["TeleportDestination"] = new SRLEProperty()).property = o2.ToString();
								}
								if (VARIABLE1.Key.ToString() == "tpsource")
								{
									(srleSaveToAdd.dictionaryWithProperties["TeleportSource"] = new SRLEProperty()).property = o2.ToString();
								}
									
									
								//Console.Log(VARIABLE1.Key + " : " + VARIABLE1.Value);	
							}
								
								
								
								
								
								
								
							if (objects.TryGetValue(objectByHashCode, out List<SRLESave> srleSave))
							{
								srleSave.Add(srleSaveToAdd);
							}
							else
							{
								objects.Add(objectByHashCode, new List<SRLESave> {srleSaveToAdd});
							}
								
						}
					}

					srleName.objects = objects;
					using (FileStream fileStream =
						new FileStream(Path.Combine(SRLEManager.Worlds.FullName, pathOfFile.Name.Replace(".world", ".srle")), FileMode.Create))
						srleName.Write(fileStream);
					SceneManager.LoadScene("MainMenu");
				}
        }  
    }

    
}