using System;
using HarmonyLib;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector))]
    internal static class Patch_AutoSaveDirector
    {
        [HarmonyPatch(nameof(AutoSaveDirector.SaveGame))]
        [HarmonyFinalizer]
        public static Exception FinalizerSaveGame(Exception __exception)
        {
            if (SaveManager.CurrentLevel == null) 
                return __exception;
            SaveManager.SaveLevel();
            return null;
        }

        [HarmonyPatch(nameof(AutoSaveDirector.OnGameLoaded))]
        [HarmonyPrefix]
        public static void OnGameLoaded()
        {
            if (LevelManager.CurrentMode == LevelManager.Mode.BUILD)
            {
                SceneContext.Instance.PlayerState.AddCurrency(1000000);
                SceneContext.Instance.PlayerState.SetEnergy(100000);
                SceneContext.Instance.PlayerState.SetHealth(100000);

                for (int i = 0; i < 10; i++)
                {
                    SceneContext.Instance.PlayerState.AddKey();
                }

                foreach(var upgrade in (PlayerState.Upgrade[])Enum.GetValues(typeof(PlayerState.Upgrade)))
                {
                    SceneContext.Instance.PlayerState.AddUpgrade(upgrade);
                }
            }
        }
    }
}