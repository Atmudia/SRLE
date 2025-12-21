using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.Adapter;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
 using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
 using Il2CppSystem;
 using Il2CppTMPro;
 using MelonLoader;
 using SR2E.Components;
 using SR2E.Managers;
 using SR2E.Popups;
 using SR2E.Storage;
 using SR2E.Utils;
 using SRLE.Components;
 using SRLE.Models;
 using SRLE.Utils;
 using UnityEngine;
 using UnityEngine.Localization.Components;
 using UnityEngine.UI;
 using Object = UnityEngine.Object;

 namespace SRLE.Patches;

 public static class Patch_MainMenu
 {
     public static bool useActiveSaveGameRootUI = false;
 }
//
// [HarmonyPatch(typeof(NewGameRootUI))]
// public static class Patch_NewGameRootUI
// {
//     [HarmonyPostfix, HarmonyPatch(nameof(NewGameRootUI.Awake))]
//     public static void Awake(NewGameRootUI __instance)
//     {
//         // if (!__instance.name.Equals("MainMenu_SRLENewGameSettings(Clone)"))
//         //     return;
//         // __instance._menuTitleHolderView.text.StringReference = LocalizationUtil.CreateByKey("UI", "b.new_level");
//         // MelonLogger.Msg(__instance._menuTitleHolderView.text.StringReference);
//         // MelonLogger.Msg("Is it setting?");
//         // return;
//     }
// }
//
[HarmonyPatch(typeof(SaveGamesRootUI))]
 public static class Patch_SaveGamesRootUI
 {
     public static TextMeshProUGUI summaryText;
     public static SaveGamesRootUI ui;
     private static List<string> levels = new List<string>();
     private static Dictionary<string,GameIconDefinition> gameIcons;
     private static string _NewIconReference = "";
     private static string _NewLevelNameReference = "";

     public static Sprite GetGameIcon(string referenceID)
     {
         
         if (referenceID==null||gameIcons == null) return UnityEUtil.Get<GameIconDefinition>("GameIconSlimeTwin").GameIcon;
         foreach (var pair in gameIcons) 
         {
             if (pair.Key.ToUpper() == referenceID.ToUpper()) return pair.Value.GameIcon;
         }
         return UnityEUtil.Get<GameIconDefinition>("GameIconSlimeTwin").GameIcon;
     }

     static string _FormatGameIcon(string name) => $"<link=\"action:select_icon\"><color=#2C6EC8><u>{name.Replace("gameIcon_","")}</u></color></link>";
     static string _FormatLevelName(string name) => $"<link=\"action:select_name\"><color=#2C6EC8><u>{name}</u></color></link>";
     public static void ChangeGameIcon()
     { 
         if (summaryText == null) return;
         var dict = new TripleDictionary<string, string, Sprite>();
         foreach (var pair in gameIcons)
             dict.Add(pair.Key,(pair.Value.name.Replace("GameIcon",""),pair.Value.GameIcon));
         SR2EGridMenuList.Open(dict, (System.Action<string>)((value) =>
         {
             _NewIconReference = value;
             RefreshSummary();
         }));
     }

     public static void ChangeLevelName()
     {
         
     }
     public static void RefreshSummary()
     {
         summaryText.text = "Create a new save!\n" +
                            "Icon: "+_FormatGameIcon(_NewIconReference)+"\n" +
                            "Level name: "+_FormatLevelName(_NewLevelNameReference)+"\n" +
                            "";
     }
     public static void OnSelect(int buttonIndex)
     {
         if (summaryText == null) return;
         if (buttonIndex == levels.Count)
         {
             RefreshSummary();
             return;
         }
         var path = levels[buttonIndex];
         var levelInfo = JsonSerializer.Deserialize<LevelData>(File.ReadAllText(path));
         summaryText.text = "Level: "+levelInfo.LevelName+"\n";
         
     }

     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.OnItemSelect))]
     public static bool OnItemSelect(SaveGamesRootUI __instance)
     {
         if(Skip(__instance)) return true;
         return false;
     }
     public static bool Skip(SaveGamesRootUI __instance)
     {
         if (Patch_MainMenu.useActiveSaveGameRootUI)
         {
             Patch_MainMenu.useActiveSaveGameRootUI = false;
             __instance.name = "MainMenu_SRLESaveGames(Clone)";
             ui = __instance;
         }
         if (__instance.name.Equals("MainMenu_SRLESaveGames(Clone)"))
             return false;
         return true;
     }
     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.FetchGameSummaryItemData))]
     public static bool FetchGameSummaryItemData(SaveGamesRootUI __instance)
     {
         if(Skip(__instance)) return true;
         return false;
     }
     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.DeleteSave))]
     public static bool DeleteSave(SaveGamesRootUI __instance)
     {
         if(Skip(__instance)) return true;
         return false;
     }
     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.CreateBehaviorModels))]
     public static bool FetchButtonBehaviorData(SaveGamesRootUI __instance, ref Il2CppReferenceArray<ButtonBehaviorModel> __result )
     {
         if(Skip(__instance)) return true;
         levels.Clear();
         _NewIconReference = "gameIcon_TwinSlime";
         _NewLevelNameReference = DateTime.Now.ToString("yyyyMMddHHmmssffff");
         var buttonBehaviorModels = new List<ButtonBehaviorModel>();
         var files = Directory.GetFiles(SaveManager.LevelsPath, "*.srle");
         gameIcons = new Dictionary<string, GameIconDefinition>();
         foreach (var def in UnityEUtil.GetAny<GameIconDefinitionCollection>().items) gameIcons.Add(def.ReferenceId,def);
         for (int i = 0; i < files.Length; i++)
         {
             var file = files[i];
             levels.Add(file);
             var levelInfo = JsonSerializer.Deserialize<LevelData>(File.ReadAllText(file));
             var translation = SR2ELanguageManger.AddTranslation(/* "Load " +*/ levelInfo.LevelName, "b.level_" + levelInfo.LevelName.GetHashCode(), "UI");
             var definition = ScriptableObject.CreateInstance<CustomSaveItemDefiniton>();
             definition = ScriptableObject.CreateInstance<CustomSaveItemDefiniton>();
             definition._label = translation;
             definition.name = translation.TableEntryReference.Key;
             definition._icon = GetGameIcon(levelInfo.LevelIcon);
             definition.hideFlags |= HideFlags.HideAndDontSave;
             definition.customAction = (System.Action)(() =>
             {
                 SaveManager.LoadLevel(file);
             });
             var model = definition.CreateButtonBehaviorModel().Cast<OpenDisplayBehaviorModel>();
             buttonBehaviorModels.Add(model);
         }
         //This just adds a new save file creator thingy to the end;
         bool addNewSave = true;
         if (addNewSave)
         {
             var translation = SR2ELanguageManger.AddTranslation("Create level", "b.level_create_new", "UI");
             var definition = ScriptableObject.CreateInstance<CustomSaveItemDefiniton>();
             definition = ScriptableObject.CreateInstance<CustomSaveItemDefiniton>();
             definition._label = translation;
             definition.name = translation.TableEntryReference.Key;
             definition._icon = __instance._emptySlotBehavior._icon;
             definition.hideFlags |= HideFlags.HideAndDontSave;
             definition.customAction = (System.Action)(() =>
             {
                 SaveManager.CreateLevel(_NewLevelNameReference,_NewIconReference);
             });
             var model = definition.CreateButtonBehaviorModel().Cast<OpenDisplayBehaviorModel>();
             buttonBehaviorModels.Add(model);
         }
         __result = buttonBehaviorModels.ToArray();
         ActionsEUtil.ExecuteInTicks((System.Action)(() =>
         {
             var title = __instance.gameObject.GetObjectRecursively<MenuTitleViewHolder>("AutoExpandingTitleBar").gameObject
                 .GetObjectRecursively<TextMeshProUGUI>("Text");
             title.text = "SRLE";
             if (summaryText != null)
                 summaryText.font = title.font;
         }),1);
         if (summaryText == null)
         {
             var detailsView = __instance.gameObject.GetObjectRecursively<GameObject>("DetailsView");
             var content = detailsView.GetObjectRecursively<Transform>("Content");
             content.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
             content.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = true;
             var obj = new GameObject();
             obj.transform.parent = content;
             obj.transform.localScale = Vector3.one;
             obj.transform.localPosition = Vector3.zero;
             obj.transform.localRotation = Quaternion.identity;
             obj.name= "SummaryText";
             summaryText = obj.AddComponent<TextMeshProUGUI>();
             var clickableTextLink = obj.AddComponent<ClickableTextLink>();
             clickableTextLink.actions.Add("select_icon",(System.Action)((() => ChangeGameIcon())));
             clickableTextLink.actions.Add("select_name",(System.Action)((() => ChangeLevelName())));
             summaryText.text = "No Text";
             //summaryText.alignment = TextAlignmentOptions.Center;
             summaryText.autoSizeTextContainer = true;
             summaryText.color = Color.black;
             summaryText.fontSize = 28;
         }
         // ButtonBehaviorModel buttonBehaviorModel = __instance._emptySlotBehavior.CreateButtonBehaviorModel();
         //var openDisplayItemDefinition = (buttonBehaviorModel._Definition_k__BackingField = 
         //    Object.Instantiate(Resources.FindObjectsOfTypeAll<OpenDisplayItemDefinition>().First()).Cast<OpenDisplayItemDefinition>()).Cast<OpenDisplayItemDefinition>();
         //openDisplayItemDefinition.name = "MainMenu_SRLENewSaveGame";
         //openDisplayItemDefinition. = LocalizationUtil.CreateByKey("UI", "b.new_level");
         //buttonBehaviorModel._Definition_k__BackingField=openDisplayItemDefinition;
         //var createNewGameScreenItemDefinition = buttonBehaviorModel._Definition_k__BackingField.Cast<CreateNewGameScreenItemDefinition>();
         //var prefab = createNewGameScreenItemDefinition.prefabToSpawn = PrefabUtils.CopyPrefab(createNewGameScreenItemDefinition.prefabToSpawn);
        // prefab.name = "MainMenu_SRLENewGameSettings";
        // var newGameRootUI = prefab.GetComponent<NewGameRootUI>();
         //newGameRootUI._menuTitleHolderView.text.StringReference = LocalizationUtil.CreateByKey("UI", "b.new_level");
         // prefab
          //prefab
       //   buttonBehaviorModel._Definition_k__BackingField.Cast<CreateNewGameScreenItemDefinition>().
        //  buttonBehaviorModels.Add(buttonBehaviorModel);
          //var buttonBehaviorModel = __instance._emptySlotBehavior.CreateButtonBehaviorModel();
          //buttonBehaviorModel.InvokeBehavior();
       //  buttonBehaviorModels.Add(buttonBehaviorModel);
         
         
        //  buttonBehaviorModel
         return false;
     }
 }
//
//
// [HarmonyPatch(typeof(MainMenuLandingRootUI))]
// public static class Patch_MainMenuLandingRootUI
// {
//     public static GameObject PrefabToSpawn;
//     public static CreateNewUIItemDefinition Definition;
//     [HarmonyPatch(nameof(MainMenuLandingRootUI.CreateModels)), HarmonyPrefix]
//     public static void CreateModels(MainMenuLandingRootUI __instance)
//     {
//         if (!PrefabToSpawn)
//         {
//             PrefabToSpawn = PrefabUtils.CopyPrefab(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(x => x.name.Equals("MainMenu_SaveGames")));
//             PrefabToSpawn.name = "MainMenu_SRLESaveGames";
//         }
//         if (Definition != null)
//         {
//             if (__instance._continueGameConfig.items.Contains(Definition))
//                 return;
//             __instance._continueGameConfig.items.Insert(2 + 1, Definition);
//             __instance._existingGameNoContinueConfig.items.Insert(2, Definition);
//             __instance._newGameConfig.items.Insert(2, Definition);
//             return;
//         }
//         Definition = ScriptableObject.CreateInstance<CreateNewUIItemDefinition>();
//         Definition.label = LocalizationUtil.CreateByKey("UI", "b.srle");
//         Definition.name = "SRLE";
//         Definition.icon = null;
//         Definition.hideFlags |= HideFlags.HideAndDontSave;
//         Definition.prefabToSpawn = PrefabToSpawn;
//         __instance._continueGameConfig.items.Insert(2 + 1, Definition);
//         __instance._existingGameNoContinueConfig.items.Insert(2, Definition);
//         __instance._newGameConfig.items.Insert(2, Definition);    
//     }
// }