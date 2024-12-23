// using System.Collections.Generic;
// using System.Linq;
// using HarmonyLib;
// using Il2CppInterop.Runtime.InteropTypes.Arrays;
// using Il2CppMonomiPark.SlimeRancher.Script.Util;
// using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
// using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
// using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
// using Il2CppSystem;
// using MelonLoader;
// using SRLE.Utils;
// using UnityEngine;
// using UnityEngine.Localization.Components;
// using Object = UnityEngine.Object;
//
// namespace SRLE.Patches;
//
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
// [HarmonyPatch(typeof(SaveGamesRootUI))]
// public static class Patch_SaveGamesRootUI
// {
//     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.FetchGameSummaryItemData))]
//     public static bool FetchGameSummaryItemData(SaveGamesRootUI __instance)
//     {
//         if (!__instance.name.Equals("MainMenu_SRLESaveGames(Clone)"))
//             return true;
//         return false;
//     }
//     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.DeleteSave))]
//     public static bool DeleteSave(SaveGamesRootUI __instance)
//     {
//         if (!__instance.name.Equals("MainMenu_SRLESaveGames(Clone)"))
//             return true;
//         return false;
//     }
//     [HarmonyPrefix, HarmonyPatch(nameof(SaveGamesRootUI.CreateBehaviorModels))]
//     public static bool FetchButtonBehaviorData(SaveGamesRootUI __instance, ref Il2CppReferenceArray<ButtonBehaviorModel> __result )
//     {
//         if (!__instance.name.Equals("MainMenu_SRLESaveGames(Clone)"))
//             return true;
//         List<ButtonBehaviorModel> buttonBehaviorModels = [];
//         ButtonBehaviorModel buttonBehaviorModel = __instance._emptySlotBehavior.CreateButtonBehaviorModel();
//         CreateNewGameScreenItemDefinition createNewGameScreenItemDefinition = (buttonBehaviorModel._Definition_k__BackingField = Object.Instantiate(Resources.FindObjectsOfTypeAll<CreateNewGameScreenItemDefinition>().First()).Cast<CreateNewGameScreenItemDefinition>()).Cast<CreateNewGameScreenItemDefinition>();
//         createNewGameScreenItemDefinition.name = "MainMenu_SRLENewSaveGame";
//         createNewGameScreenItemDefinition._titleText = LocalizationUtil.CreateByKey("UI", "b.new_level");
//         // var createNewGameScreenItemDefinition = buttonBehaviorModel._Definition_k__BackingField.Cast<CreateNewGameScreenItemDefinition>();
//         var prefab = createNewGameScreenItemDefinition.prefabToSpawn = PrefabUtils.CopyPrefab(createNewGameScreenItemDefinition.prefabToSpawn);
//         prefab.name = "MainMenu_SRLENewGameSettings";
//         var newGameRootUI = prefab.GetComponent<NewGameRootUI>();
//         newGameRootUI._menuTitleHolderView.text.StringReference = LocalizationUtil.CreateByKey("UI", "b.new_level");
//         // prefab
//         // prefab
//         // buttonBehaviorModel._Definition_k__BackingField.Cast<CreateNewGameScreenItemDefinition>().
//         // buttonBehaviorModels.Add(buttonBehaviorModel);
//         // var buttonBehaviorModel = __instance._emptySlotBehavior.CreateButtonBehaviorModel();
//         // buttonBehaviorModel.InvokeBehavior();
//         buttonBehaviorModels.Add(buttonBehaviorModel);
//         
//         __result = buttonBehaviorModels.ToArray();
//         
//         // buttonBehaviorModel
//         return false;
//     }
// }
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