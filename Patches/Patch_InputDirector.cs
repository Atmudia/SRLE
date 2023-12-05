using System.Linq;
using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.Input;
using MelonLoader;
using SRLE.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace SRLE.Patches;

[HarmonyPatch(typeof(InputDirector), nameof(InputDirector.Update))]
public class Patch_InputDirector
{
    public static bool Prefix(InputDirector __instance)
    {
        if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
        {
            if (__instance._screenshot.triggered)
            {
                SRSingleton<GameContext>.Instance.TakeScreenshot();
            }
            __instance._mainGame.Disable();
            __instance._paused.Enable();
            return false;
        }
        return true;
    }
}