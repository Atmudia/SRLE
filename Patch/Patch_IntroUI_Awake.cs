using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(IntroUI), nameof(IntroUI.Awake))]
    public static class Patch_IntroUI_Awake
    {
        public static bool Prefix(IntroUI __instance)
        {
            if (SRLEManager.isSRLELevel)
            {
                Destroyer.Destroy(__instance.gameObject, "#SRLE.IntroUI_Awake");
                return false;
            }

            return true;
        }
    }
}