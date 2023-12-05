using HarmonyLib;
using Il2CppMonomiPark.SlimeRancher.World.Teleportation;
using SRLE.Components;

namespace SRLE.Patches;

[HarmonyPatch(typeof(StaticTeleporterNode), nameof(StaticTeleporterNode.NodeId), MethodType.Getter)]
public static class Patch_StaticTeleportNode
{
    public static void Postfix(StaticTeleporterNode __instance, ref string __result)
    {
        var componentInParent = __instance.GetComponentInParent<BuildObjectId>();
        if (componentInParent == null) return;
        if (componentInParent.buildObject.Properties.TryGetValue("TeleporterNode", out var property))
        {
            __result = property;
        }
    }
}