﻿using HarmonyLib;
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

        if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
        {
            
            SRLESaveSystem.SaveLevel(SRLEMod.CurrentLevelPath);
            //World.Save();
            return false;
        }
        return true;
    }

    [HarmonyPatch(nameof(AutoSaveDirector.OnGameLoaded))]
    [HarmonyPrefix]
    public static void OnGameLoaded()
    {
        
        
        if (SRLEMod.CurrentMode == SRLEMod.Mode.BUILD)
        {


        }
    }
}