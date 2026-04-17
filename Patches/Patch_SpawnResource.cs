using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRLE.Components;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(SpawnResource))]
    public class Patch_SpawnResource
    {
        [HarmonyPatch(nameof(SpawnResource.Update)), HarmonyPrefix]
        public static bool Update(SpawnResource __instance)
        {
            if (LevelManager.IsActive)
                if (ObjectManager.GetBuildObject(__instance.gameObject, out _))
                {
                    __instance.UpdateToTime(__instance.timeDir.WorldTime(), __instance.timeDir.DeltaWorldTime());
                    if (__instance.spawnQueue.Count > 0)
                    {
                        __instance.Spawn(__instance.spawnQueue.Dequeue());
                    }
                    return false;   
                }
            return true;
        }

        [HarmonyPatch(nameof(SpawnResource.Spawn), typeof(IEnumerable<SpawnResource.SpawnMetadata>)), HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Spawn(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            var targetGetSetId = AccessTools.PropertyGetter(typeof(Region), "setId");
            var region = AccessTools.Field(typeof(SpawnResource), nameof(SpawnResource.region));

            
            for (int i = 0; i < code.Count - 2; i++)
            {
                if (code[i].opcode == OpCodes.Ldarg_0 &&
                    code[i + 1].OperandIs(region) &&
                    code[i + 2].Calls(targetGetSetId))
                {
                    code[i] = new CodeInstruction(OpCodes.Ldarg_0);
                    code[i + 1] = CodeInstruction.Call(typeof(Patch_SpawnResource), nameof(GetCustomRegionSetId));
                    code.RemoveAt(i + 2);
                    break;
                }
            }


            return code;
        }

        public static RegionRegistry.RegionSetId GetCustomRegionSetId(SpawnResource self)
        {
            if (LevelManager.IsActive)
                if (ObjectManager.GetBuildObject(self.gameObject, out BuildObject buildObject))
                {
                    return buildObject.Region;
                }
            return self.region.setId; 
        }
        
    }
}