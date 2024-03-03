using System.Collections.Generic;
using HarmonyLib;

namespace SRLE.Utils;

public static class Extensions
{
    public static T GetField<T>(this object @this, string name)
    {
        var type = @this.GetType();
        return (T)type.GetField(name, AccessTools.all).GetValue(@this);
    }
    public static T GetProperty<T>(this object @this, string name)
    {
        var type = @this.GetType();
        return (T)type.GetProperty(name, AccessTools.all).GetValue(@this);
    }

    public static bool EqualsInIL2CPP(this Il2CppSystem.Object a, Il2CppSystem.Object b)
    {
        return Il2CppSystem.Object.Equals(a, b);
    }
    public static bool ContainsKeyInIL2CPP<Key, Value>(this Dictionary<Key, Value> a, Il2CppSystem.Object b) where Key : Il2CppSystem.Object
    {
        foreach (var value in a)
        {
            return EqualsInIL2CPP(value.Key, b);
        }
        return false;
    }
}