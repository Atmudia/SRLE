using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(BaseUI), nameof(BaseUI.OnCancelPressed))]
    public class Patch_BaseUI_OnCancelPressed
    {
        public static bool Prefix(BaseUI __instance)
        {
            if (__instance is not ExpoGameSelectUI && __instance.name != "SRLE_UI") return true;
            SRLEUIMenu.returnToMenu = true;
            __instance.Close();
            return false;
        }
    }
}