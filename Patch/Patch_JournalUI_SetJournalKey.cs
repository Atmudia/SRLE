using HarmonyLib;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(JournalUI), nameof(JournalUI.SetJournalKey))]
    public static class Patch_JournalUI_SetJournalKey
    {
        public static bool Prefix(JournalUI __instance, string journalKey)
        {
            if (journalKey.Contains("srle."))
            {
                __instance.journalText.text = journalKey.Replace("srle.", string.Empty);
                
                return false;
            }

            return true;
        }
    }
}