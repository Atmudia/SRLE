using System;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace SRLE.Patches;

[HarmonyPatch(typeof(ClassInjector), "GetIl2CppTypeFullName")]
public class Patch_ClassInjector
{
    public static void Postfix(ref string __result)
    {
        var typeByName = AccessTools.TypeByName(__result);
        if (typeByName == null)
        {
            __result = "Il2Cpp" + __result;
        }
    }
}