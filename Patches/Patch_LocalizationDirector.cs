using System.Collections;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.Localization;
using MelonLoader;
using UnityEngine;

namespace SRLE
{
    [HarmonyPatch(typeof(LocalizationDirector), nameof(LocalizationDirector.LoadTables))]
    public static class Patch_LocalizationDirector
    {
        public static void Postfix(LocalizationDirector __instance) => MelonCoroutines.Start(LaunchLocalization(__instance));

        public static IEnumerator LaunchLocalization(LocalizationDirector director)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            director.Tables["UI"].AddEntry("b.srle", "SRLE");
        }
    }
}