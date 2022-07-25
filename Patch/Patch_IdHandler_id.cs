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
            if (__instance.gameObject.TryGetComponent(out ObjectAddedBySRLE _component))
            {
                __result = __instance.IdPrefix() + idOfIdHandler;
                idOfIdHandler++;
                return false;
            }
            return true;
        }

        private static ulong idOfIdHandler = 0;
    }
    
}