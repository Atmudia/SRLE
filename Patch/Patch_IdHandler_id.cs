using System;
using HarmonyLib;
using SRLE.Components;

namespace SRLE.Patch
{
    [HarmonyPatch(typeof(IdHandler), nameof(IdHandler.id), MethodType.Getter)]
    public static class Patch_IdHandler_id
    {
        public static bool Prefix(IdHandler __instance, ref string __result)
        {
            foreach (var objectAddedBySrle in __instance.gameObject.GetComponentsInParent<ObjectAddedBySRLE>())
            {
                EntryPoint.SRLEConsoleInstance.Log(objectAddedBySrle.name);
                __result = __instance.IdPrefix() + idOfIdHandler;
                idOfIdHandler++;
                return false;
            }
            return true;
        }

        private static ulong idOfIdHandler = 0;
    }
    
}