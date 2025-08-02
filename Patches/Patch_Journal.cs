using HarmonyLib;
using SRLE.Components;
using UnityEngine;

namespace SRLE.Patches
{
    [HarmonyPatch]
    public static class Patch_Journal
    {
        const string CustomPrefix = "custom=";
        [HarmonyPatch(typeof(JournalEntry), nameof(JournalEntry.Activate)), HarmonyPrefix]
        public static bool Activate(JournalEntry __instance, ref GameObject __result)
        {
            if (__instance.GetComponent<BuildObject>())
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.uiPrefab);
                gameObject.GetComponent<JournalUI>().SetJournalKey(CustomPrefix + __instance.entryKey);
                foreach (ProgressDirector.ProgressType type in __instance.ensureProgress)
                {
                    SRSingleton<SceneContext>.Instance.ProgressDirector.SetProgress(type, 1);
                }
                __result = gameObject;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(JournalUI), nameof(JournalUI.SetJournalKey)), HarmonyPrefix]
        public static bool SetJournalKey(JournalUI __instance, string journalKey)
        {
            if (journalKey.StartsWith(CustomPrefix))
            {
                __instance.journalText.text = journalKey.Substring(7);
                return false;
            }
            return true;
        }
    }
}