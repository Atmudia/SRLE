using System;
using HarmonyLib;
using MelonLoader;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(AutoSaveDirector))]
internal static class Patch_AutoSaveDirector
{
    [HarmonyPatch(nameof(AutoSaveDirector.SaveGame))]
    [HarmonyPrefix]
    public static bool SaveGame()
    {
        return LevelManager.CurrentMode != LevelManager.Mode.BUILD;
    }

    [HarmonyPatch(nameof(AutoSaveDirector.SaveGame))]
    [HarmonyFinalizer]
    public static Exception FinalizerSaveGame(Exception __exception)
    {
        if (LevelManager.CurrentMode != LevelManager.Mode.BUILD) return __exception;
        SaveManager.SaveLevel();
        return null;
    }

    [HarmonyPatch(nameof(AutoSaveDirector.OnGameLoaded))]
    [HarmonyPrefix]
    public static void OnGameLoaded()
    {
        if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
        {


        }
    }
}