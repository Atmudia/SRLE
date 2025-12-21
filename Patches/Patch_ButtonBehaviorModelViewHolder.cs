using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using SR2E.Utils;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(ButtonBehaviorViewHolder), nameof(ButtonBehaviorViewHolder.OnSelected))]
internal static class SaveButtonPatch
{
    internal static void Postfix(ButtonBehaviorViewHolder __instance)
    {
        if (__instance.gameObject.name != "SaveGameSlotButton(Clone)") return;
        if (Patch_SaveGamesRootUI.ui == null) return;
        
        //There are inactive preview buttons for some reason...
        //This is to ensure it works if they remove them or change their count
        int activeIndex = 0;
        int activeChildCount = 0;
        foreach (var child in __instance.transform.parent.GetChildren())
        {
            activeChildCount++;
            if (!child.gameObject.activeSelf) continue;

            if (child == __instance.transform)
                break;

            activeIndex++;
        }
        if (activeIndex >= activeChildCount) return;
        Patch_SaveGamesRootUI.OnSelect(activeIndex);
    }
}