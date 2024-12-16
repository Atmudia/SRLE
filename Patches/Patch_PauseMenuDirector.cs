using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Script.UI.Pause;
using MelonLoader;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(PauseMenuDirector))]
internal static class Patch_PauseMenuDirector
{
    [HarmonyPatch(nameof(PauseMenuDirector.PauseGame)), HarmonyPrefix]
    
    public static bool TogglePause(PauseMenuDirector __instance)
    {
        
        if (LevelManager.CurrentMode != LevelManager.Mode.BUILD) return true;
        SRLECamera.Instance.SetActive(!SRLECamera.Instance.isActiveAndEnabled);
        MelonLogger.Msg(!SRLECamera.Instance.isActiveAndEnabled);
        return false;
    }
    [HarmonyPatch(nameof(PauseMenuDirector.Quit)), HarmonyPrefix]
    public static void Quit(PauseMenuDirector __instance)
    {
        if (LevelManager.CurrentMode != LevelManager.Mode.BUILD) return;
        SRLECamera.Instance.SetActive(false);
        MelonCoroutines.Stop(SRLECamera.Instance.DestroyDelayedObject);
        SRSingleton<GameContext>.Instance.InputDirector._paused.Map.Enable();
        UnityEngine.Object.Destroy(LevelManager.SRLEGameObject);
        UnityEngine.Object.Destroy(ObjectManager.World);
        UnityEngine.Object.Destroy(ObjectManager.CachedGameObjects);
        ObjectManager.BuildObjects.Clear();
        // SRSingleton<GameContext>.Instance.InputDirector._mainGame.Map.Enable();


    }

}