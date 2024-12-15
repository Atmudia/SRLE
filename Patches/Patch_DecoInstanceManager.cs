using System.Collections.Generic;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace SRLE.Patches;

[HarmonyPatch(typeof(DecoInstanceManager), nameof(DecoInstanceManager.DrawDecoMeshes))]
public class Patch_DecoInstanceManager
{
    public static void Prefix(DecoInstanceManager __instance, List<Camera> cameras)
    {
    }
}