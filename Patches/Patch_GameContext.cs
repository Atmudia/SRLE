using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppMonomiPark.SlimeRancher.Script.Util;
using Il2CppMonomiPark.SlimeRancher.UI.ButtonBehavior;
using Il2CppSystem.Linq;
using SR2E.Buttons;
using SR2E.Menus;
using SR2E.Popups;
using SR2E.Utils;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(GameContext), nameof(GameContext.Start))]
public class Patch_GameContext
{
    public static void Postfix(GameContext __instance)
    {
        var label = LocalizationUtil.CreateByKey("UI", "b.srle");
        new CustomMainMenuButton(label, EmbeddedResourceEUtil.LoadSprite("icon.png"), 4, (System.Action)(() =>
        {
            Patch_MainMenu.useActiveSaveGameRootUI = true;
            var def = UnityEUtil.Get<OpenDisplayItemDefinition>("SaveFilesDisplay");
            def.CreateButtonBehaviorModel().InvokeBehavior();
            
        }));
        
        GameObject manager = new GameObject("SRLE");
        manager.AddComponent<SRLEMod>();
        GameObject.DontDestroyOnLoad(manager);
        try
        {
            var summariesToDelete = new List<Summary>();
            foreach (var summary in __instance.AutoSaveDirector.EnumerateAllSaveGamesIncludingBackups().ToList())
                if(summary.SaveSlotIndex==SaveManager.UsedSaveSlotIndex)
                    summariesToDelete.Add(summary);
            foreach (var summary in summariesToDelete)
            {
                try
                {
                    __instance.AutoSaveDirector.DeleteGame(summary.Name);
                    __instance.AutoSaveDirector._storageProvider.DeleteGameData(summary.SaveName);
                } catch { }
            }
        } catch { }
    }
}