using SR2E.Managers;
using UnityEngine;

namespace SRLE;

public static class LevelManager
{
    public enum Mode
    {
        NONE,
        TEST,
        BUILD
    }

    public static Mode CurrentMode { get; private set; }
    public static bool IsLoading = false;
    public static GameObject SRLEGameObject;

    public static void SetMode(Mode mode)
    {
        if(mode==Mode.NONE)
            SR2ECounterGateManager.DeregisterFor_PlayerCameraDisableUseOcclusionCulling(EntryPoint.instance);
        else SR2ECounterGateManager.RegisterFor_PlayerCameraDisableUseOcclusionCulling(EntryPoint.instance);
        CurrentMode = mode; 
    }
}