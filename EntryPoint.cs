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
    public class EntryPoint : MelonMod
    {
        public const string Version = "1.0.0";

        public override void OnEarlyInitializeMelon()
        {
        }

        public override void OnInitializeMelon()
        {
            foreach (var type in AccessTools.GetTypesFromAssembly(typeof(EntryPoint).Assembly))
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            GameObject manager = new GameObject("SRLE");
            manager.AddComponent<SRLEMod>();
            manager.hideFlags |= HideFlags.HideAndDontSave;
        }
    }
}


namespace System.Runtime.CompilerServices
{
    public class IsUnmanagedAttribute : Attribute {
    } 
}