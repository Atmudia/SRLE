using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu.Model;
using Il2CppTMPro;
using MelonLoader;
using SR2E.Utils;
using SRLE.Components;
using UnityEngine.UI;

namespace SRLE.Patches;

[HarmonyPatch(typeof(OpenDisplayBehaviorModel), nameof(OpenDisplayBehaviorModel.InvokeBehavior))]
public class Patch_OpenDisplayBehaviorModel
{
    internal static bool Prefix(OpenDisplayBehaviorModel __instance)
    {
        if (__instance.Definition is CustomSaveItemDefiniton definition)
        {
            definition.customAction.Invoke();
            return false;
        }
        return true;
    }
}
