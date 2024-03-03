global using Il2Cpp;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
// using RuntimeHandle;
using SRLE;
using SRLE.Components;
using UnityEngine;
using Object = Il2CppSystem.Object;

[assembly: MelonInfo(typeof(EntryPoint), "SRLE", EntryPoint.Version, "SRLE Contributors")]
namespace SRLE;


// [HarmonyPatch(typeof(LandPlotModel), nameof(LandPlotModel.Pull))]
// public static class Patch
// {
//     public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
//     {
//         var parameterInfos = __originalMethod.GetParameters().ToList();
//         parameterInfos.Insert(0, null);
//         var codeInstructions = instructions.ToArray();
//         for (var index = 0; index < codeInstructions.Length; index++)
//         {
//             var codeInstruction = codeInstructions[index];
//             if (codeInstruction.opcode == OpCodes.Ldarg)
//             {
//                 var findNextElement = codeInstructions.FindNextElement(index, instruction => instruction.opcode == OpCodes.Newobj);
//                 ConstructorInfo constructorInfo = (ConstructorInfo)findNextElement.operand;
//                 if (!constructorInfo.DeclaringType.IsGenericTypeDefinition)
//                 {
//                     continue;
//                 }
//                 ParameterInfo parameterInfo = parameterInfos[(short)codeInstruction.operand];
//                 var makeGenericType = constructorInfo.DeclaringType.MakeGenericType(parameterInfo.ParameterType.GetGenericArguments()).GetConstructor(new Type[]
//                 {
//                     typeof(IntPtr)
//                 });
//                 findNextElement.operand = makeGenericType;
//             }
//             
//         }
//         return codeInstructions;
//     }
//
//     public static T FindNextElement<T>(this T[] array, int currentIndex, Func<T, bool> predicate)
//     {
//
//         if (currentIndex != -1 && currentIndex < array.Length - 1)
//         {
//             for (int i = currentIndex + 1; i < array.Length; i++)
//             {
//                 if (predicate(array[i]))
//                 {
//                     return array[i];
//                 }
//             }
//         }
//
//         // If the current element is not found or is the last element, return default value
//         return default;
//     }
//
// }

public class EntryPoint : MelonMod
{
    public const string Version = "1.0.0";


    public override void OnInitializeMelon()
    { 
        foreach (var type in AccessTools.GetTypesFromAssembly(typeof(EntryPoint).Assembly))
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
        GameObject manager = new GameObject("SRLE");
        manager.AddComponent<SRLEMod>();
        manager.hideFlags |= HideFlags.HideAndDontSave;
        
        
        // ClassInjector.RegisterTypeInIl2Cpp<RuntimeTransformHandle>();
        // ClassInjector.RegisterTypeInIl2Cpp<HandleBase>();
        // ClassInjector.RegisterTypeInIl2Cpp<ScaleGlobal>();
        // ClassInjector.RegisterTypeInIl2Cpp<ScaleAxis>();
        // ClassInjector.RegisterTypeInIl2Cpp<ScaleHandle>();
        //
        // ClassInjector.RegisterTypeInIl2Cpp<RotationAxis>();
        // ClassInjector.RegisterTypeInIl2Cpp<RotationHandle>();
        //
        // ClassInjector.RegisterTypeInIl2Cpp<PositionAxis>();
        // ClassInjector.RegisterTypeInIl2Cpp<PositionHandle>();
        // ClassInjector.RegisterTypeInIl2Cpp<PositionPlane>();






        

    }

    public override void OnSceneWasInitialized(int level, string sceneName)
    {
    }
        
}