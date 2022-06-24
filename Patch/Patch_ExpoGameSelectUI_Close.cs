using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(ExpoGameSelectUI), nameof(ExpoGameSelectUI.Close))]
    public class Patch_ExpoGameSelectUI_Close
    {
        public static bool Prefix(ExpoGameSelectUI __instance)
        {
            if (__instance.gameObject.name != "SRLE_UI") return true;
            __instance.mainMenuUIPrefab.SetActive(true);
            Destroyer.Destroy(__instance.gameObject, "BaseUI.Close");
            return false;
        }
    }
}