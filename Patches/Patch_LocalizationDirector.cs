using System.Collections;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using MelonLoader;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.LoadTables))]
public static class Patch_LocalizationDirector
{
    public static void Postfix(LocalizationDirector __instance)
    {
        MelonCoroutines.Start(LoadTable(__instance));
    }

    public static IEnumerator LoadTable(LocalizationDirector localizationDirector)
    {
        yield return new WaitForSeconds(0.1f);
        localizationDirector.Tables["UI"].AddEntry("b.srle", "SRLE");
        localizationDirector.Tables["UI"].AddEntry("b.new_level", "NEW LEVEL");
    }
}