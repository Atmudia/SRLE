using HarmonyLib;
using SRLE.Components;
using SRML.SR.SaveSystem;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(IdHandler))]
    public class Patch_IdHandler
    {
        [HarmonyPatch("id", MethodType.Getter), HarmonyPrefix]
        public static bool GetId(IdHandler __instance, ref string __result)
        {
            if (ObjectManager.GetBuildObject(__instance.gameObject, out var buildObj))
            {
                if (buildObj.IdHandler == 0) 
                    buildObj.IdHandler = BuildObject.GlobalIdHandler++;
                __result = $"SRLE.{__instance.IdPrefix()}.{buildObj.IdHandler}";
                EntryPoint.ConsoleInstance.Log($"Overwrote id with {__result}");
                return false;
            }
            return true;
        }
    }
}