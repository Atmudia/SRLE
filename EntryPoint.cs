global using Il2Cpp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppMonomiPark.SlimeRancher.UI.Adapter;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using MelonLoader.NativeUtils;
// using RuntimeHandle;
using SRLE;
using SRLE.Components;
using SRLE.Utils;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using NativeLibrary = MelonLoader.NativeLibrary;
using Object = Il2CppSystem.Object;
[assembly: MelonInfo(typeof(EntryPoint), "SRLE", EntryPoint.Version, "SRLE Contributors")]
namespace SRLE
{
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

        public override void OnEarlyInitializeMelon()
        {
            // IL2CPP.il2cpp_add_internal_call();
            // IL2CPP.il2cpp_add_internal_call();
            // IntPtr from = NativeLibrary.Load("GameAssembly.dll").GetExport("il2pp_");
            // deleg = new RuntimeInvokeDetourDelegate(OnInvokeMethod);
            // hook = new NativeHook<RuntimeInvokeDetourDelegate>(from, Marshal.GetFunctionPointerForDelegate(deleg));
            // hook.Attach();
            // MelonLogger.Msg("is this something?");
            // base.OnEarlyInitializeMelon();
        }

        public static Delegate GetICall(Type delegateType, string signature)
        {
            IntPtr ptr = IL2CPP.il2cpp_resolve_icall(signature);
            if (ptr != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(ptr, delegateType);
            }

            throw new NullReferenceException($"Failed to get ICall: {signature}");
            return null;
        }
        public override void OnInitializeMelon()
        { 
            // IL2CPP.il2cpp_resolve_icall

            // MeshExtensions.NativeMethods.CopyAttributeIntoPtr()

            // foreach (var field in typeof(MeshExtensions.NativeMethods).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            // {
            //     var signatureAttribute = field.GetCustomAttribute<MeshExtensions.SignatureAttribute>();
            //     if (signatureAttribute != null)
            //     {
            //         var delegateType = field.FieldType;
            //         var del = GetICall(delegateType, signatureAttribute.Signature);
            //         field.SetValue(null, del);
            //     
            //     }
            // }
       
        
            // var il2CPPResolveIcall = IL2CPP.il2cpp_resolve_icall("UnityEngine.Mesh/MeshData::HasVertexAttribute");
            // var il2CPPResolveIcall = IL2CPP.il2cpp_resolve_icall("UnityEngine.Mesh/MeshData::CopyAttributeIntoPtr");
        
        
            // MelonLogger.Msg(il2CPPResolveIcall);
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
            if (sceneName.Contains("MainMenuEnvironment"))
            {
                // var nonReadableMesh = Resources.FindObjectsOfTypeAll<Mesh>().FirstOrDefault(x => !x.isReadable);
                // Mesh meshCopy = new Mesh
                // {
                //     indexFormat = nonReadableMesh.indexFormat,
                //     name = nonReadableMesh.name
                // };
                //
                // // Handle vertices
                // GraphicsBuffer verticesBuffer = nonReadableMesh.GetVertexBuffer(0);
                // int totalSize = verticesBuffer.stride * verticesBuffer.count;
                //
                // byte[] buffer = new byte[totalSize];
                // var instance = Il2CppSystem.Array.CreateInstance(Il2CppType.Of<byte>(), totalSize);
                // verticesBuffer.InternalGetData(instance, 0, 0, instance.Length, Marshal.SizeOf(instance.GetIl2CppType().GetElementType()));
                // // verticesBuffer.GetData(instance);
                // meshCopy.SetVertexBufferParams(nonReadableMesh.vertexCount, nonReadableMesh.GetVertexAttributes());
                // foreach (var @object in instance)
                // {
                //     Melon<SRLE.EntryPoint>.Logger.Msg(@object.GetIl2CppType().Name);
                //     // @object.Cast<>()
                // }

                return;
                // meshCopy.SetVertexBufferData(instance, 0, 0, totalSize);
                // verticesBuffer.Release();
                //
                // // Handle triangles
                // meshCopy.subMeshCount = nonReadableMesh.subMeshCount;
                // GraphicsBuffer indexesBuffer = nonReadableMesh.GetIndexBuffer();
                // int tot = indexesBuffer.stride * indexesBuffer.count;
                // byte[] indexesData = new byte[tot];
                // indexesBuffer.GetData(indexesData);
                // meshCopy.SetIndexBufferParams(indexesBuffer.count, nonReadableMesh.indexFormat);
                // meshCopy.SetIndexBufferData(indexesData, 0, 0, tot);
                // indexesBuffer.Release();
                //
                // // Restore submesh structure
                // uint currentIndexOffset = 0;
                // for (int i = 0; i < meshCopy.subMeshCount; i++)
                // {
                //     uint subMeshIndexCount = nonReadableMesh.GetIndexCount(i);
                //     var subMeshDescriptor = new SubMeshDescriptor
                //     {
                //         indexStart = (int)currentIndexOffset,
                //         indexCount = (int)subMeshIndexCount
                //     };
                //     meshCopy.SetSubMesh(i, subMeshDescriptor);
                //     currentIndexOffset += subMeshIndexCount;
                // }

                // Recalculate normals and bounds
                // meshCopy.RecalculateNormals();
                // meshCopy.RecalculateBounds();
                // var acquireReadOnlyMeshData = new Mesh.MeshDataArray(firstOrDefault, false);
                // var meshData = acquireReadOnlyMeshData.GetMeshData(0);
                // var verticesCount = meshData.GetVerticesCount();
                // var vertices = new NativeArray<Vector3>(verticesCount, Allocator.None);
                // meshData.GetVertices(vertices);
                // foreach (var outVertex in outVertices)
                // {
                //     MelonLogger.Msg("Normals: " +outVertex);
                // }
                //
                // var normals = new NativeArray<Vector3>(verticesCount, Allocator.Temp);
                // meshData.GetNormals(normals);
                // foreach (var outVertex in outNormals)
                // {
                //     MelonLogger.Msg("Normals:" + outVertex);
                // }
                
                // var subMeshCount = meshData.GetSubMeshCount();
                // NativeArray<ushort> triangles = new NativeArray<ushort>(subMeshCount > 0 ? meshData.GetIndexCount(0) : 0, Allocator.Temp);
                
                // // if (meshData.uvChannelCount > 0)
                // //     meshData.GetUVs(0, uvs);
                // // // Get triangles
                // if (subMeshCount > 0)
                //     meshData.GetIndices(triangles, 0);
                // using (StreamWriter sw = new StreamWriter("txt.obj"))
                // {
                //     // Write vertices
                //     foreach (var vertex in vertices)
                //     {
                //         sw.WriteLine($"v {vertex.x} {vertex.y} {vertex.z}");
                //     }
                //
                //     // Write normals
                //     foreach (var normal in normals)
                //     {
                //         sw.WriteLine($"vn {normal.x} {normal.y} {normal.z}");
                //     }
                //
                //     // Write UVs
                //     // if (uvs.Length > 0)
                //     // {
                //     //     foreach (var uv in uvs)
                //     //     {
                //     //         sw.WriteLine($"vt {uv.x} {uv.y}");
                //     //     }
                //     // }
                //
                //     // Write triangles (faces)
                //     for (int i = 0; i < triangles.Length; i += 3)
                //     {
                //         int v1 = triangles[i] + 1;
                //         int v2 = triangles[i + 1] + 1;
                //         int v3 = triangles[i + 2] + 1;
                //         sw.WriteLine($"f {v1}/{v1}/{v1} {v2}/{v2}/{v2} {v3}/{v3}/{v3}");
                //     }
                // }
                //
                // // Dispose of the NativeArrays after use
                // vertices.Dispose();
                // normals.Dispose();
                // // uvs.Dispose();
                // triangles.Dispose();
                // meshDataArray.Dispose();
            }
        }
                //
                // meshData.GetNormals()
                
                
            }
        
}


namespace System.Runtime.CompilerServices
{
    public class IsUnmanagedAttribute : Attribute {
    } 
}