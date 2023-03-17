using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Localization;

namespace SRLE
{
    [HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
    public static class MainMenuLandingRootUIInitPatch
    {
        public static void Postfix(MainMenuLandingRootUI __instance)
        {
           /* var buttonBehaviorDefinition = ScriptableObject.CreateInstance<QuitGameItemDefinition>();
            var stringTableEntry = LocalizationUtil.CreateByKey("UI", "b.srle");
            buttonBehaviorDefinition.label = stringTableEntry;
            buttonBehaviorDefinition.hideFlags |= HideFlags.HideAndDontSave;
            __instance.models.Add(new ButtonBehaviorModel(buttonBehaviorDefinition));
            __instance.gameObject.FindChild("LayoutGroup_Right (MainMenuButtons)", true).PrintComponent();
            */
        }
    }
    
}