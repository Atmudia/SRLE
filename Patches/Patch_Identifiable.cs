using System;
using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(Identifiable), "Awake")]
public static class Identifiable_Awake
{
    static void Postfix(Identifiable __instance)
    {
        
        if (SRLECamera.Instance != null && SRLECamera.Instance.isActiveAndEnabled)
        {
            if(__instance.identType != SRSingleton<SceneContext>.Instance.Player.GetComponent<Identifiable>().identType)
            {
                try
                {
                    Destroyer.DestroyActor(__instance.gameObject, "CameraActivated");
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}