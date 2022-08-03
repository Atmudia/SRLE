using System.Linq;
using HarmonyLib;
using SRML.Console;
using UnityEngine;

namespace SRLE
{
    [HarmonyPatch(typeof(ExpoGameSelectUI), nameof(ExpoGameSelectUI.Close))]
    public class Patch_ExpoGameSelectUI_Close
    {
        public static bool Prefix(ExpoGameSelectUI __instance)
        {
            if (__instance.gameObject.name == "SRLE_UI(Clone)")
            {
                Console.Log(__instance.gameObject.name);
                Destroyer.Destroy( __instance.gameObject, "BaseUI.Close");
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(BaseUI), nameof(BaseUI.OnDestroy))]
    public class Patch_BaseUI_OnDestroy
    {
        public static bool Prefix(BaseUI __instance)
        {
            if (__instance.gameObject.name == "SRLE_UI(Clone)") return false;
            return true;
        }
    }
    [HarmonyPatch(typeof(ExpoGameSelectUI), nameof(ExpoGameSelectUI.LoadGame))]
    public class Patch_ExpoGameSelectUI_LoadGame
    {
        public static bool activate;
        public static bool Prefix(ExpoGameSelectUI __instance, TextAsset asset)
        {
            if (__instance.gameObject.name == "SRLE_UI(Clone)")
            {
                if (asset.name == "AdvancedGame")
                {
                    EntryPoint.menu.InstantiateAndWaitForDestroy(LoaderAssets.NewLevelUI.gameObject);
                    Destroyer.Destroy( __instance.gameObject, "BaseUI.Close");

                }
                return false;
            }
            return true;
        }
    }
}