using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.UI.MainMenu;
using Il2CppTMPro;

namespace SRLE
{
    [HarmonyPatch(typeof(LocalizedVersionText), nameof(LocalizedVersionText.Awake))]
    public static class VersionText
    {
        public static void Postfix(LocalizedVersionText __instance)
        {
            __instance.text.text += "\nSRLE v" + EntryPoint.Version;
        }
    }
}