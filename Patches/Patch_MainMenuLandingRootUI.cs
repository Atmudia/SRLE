using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(MainMenuLandingRootUI), nameof(MainMenuLandingRootUI.CreateModels))]
    public static class Patch_MainMenuLandingRootUI
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