using HarmonyLib;
using UnityEngine;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(KillOnTrigger), nameof(KillOnTrigger.OnTriggerEnter))]
    internal static class KillOnTrigger_OnTriggerEnter
    {
        public static bool Prefix(Collider collider)
        {
            if (LevelManager.CurrentMode != LevelManager.Mode.TEST) return true;
            return !PhysicsUtil.IsPlayerMainCollider(collider);
        }
    }
}