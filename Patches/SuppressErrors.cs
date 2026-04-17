using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonomiPark.SlimeRancher;
using UnityEngine;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(SavedGame))]
    public class SuppressErrors
    {
        public static MethodInfo DebugLogMethod = AccessTools.Method(typeof(Debug), nameof(Debug.Log), new []{typeof(object)});
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(SavedGame), nameof(SavedGame.SetSpawnTimes));
            yield return AccessTools.Method(typeof(SavedGame), nameof(SavedGame.SetTriggerTimes));
            yield return AccessTools.Method(typeof(SavedGame), nameof(SavedGame.SetAnimalSpawnTimes));
        }
        
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(DebugLogMethod))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instruction;
                }
            }
        }


    }
}
