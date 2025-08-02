using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SRLE.Patches
{
    [HarmonyPatch(typeof(Debug), nameof(Debug.Log), typeof(object))]
    public class SuppressErrors
    {
        public static bool Prefix(object message)
        {
            return message is not "Skipping deserializing spawn time, as it's missing.";
        }
    }
}
