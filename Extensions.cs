using System.Collections.Generic;
using System.Linq;
using Il2CppSystem.IO;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace SRLE;

public static class Extensions
{
    public static void Log(this object @this) => MelonLogger.Msg(@this.ToString());
    public static void LogWarning(this object @this) => MelonLogger.Warning(@this.ToString());
    public static void LogError(this object @this) => MelonLogger.Error(@this.ToString());
    public static void PrintComponent(this GameObject obj)
    {
            Log(obj.name);
            foreach (Component component in obj.GetComponentsInChildren<Component>(true))
            {
                Log("   " + component.ToString());
            }
    }

    public static T LoadFromJSON<T>(this string @this) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(@this);
    public static void SaveToJSON<T>(this T @this, string path) => File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(@this, Formatting.Indented));

    public static GameObject FindChild(this GameObject obj, string name, bool dive = false)
    {
        if (!dive)
            return obj.transform.Find(name).gameObject;

        GameObject child = null;
        if (obj.transform != null)
            foreach (var o in obj.transform)
            {
                Transform transform = o.Cast<Transform>();

                if (!(transform == null))
                {
                    if (transform.name.Equals(name))
                    {
                        child = transform.gameObject;
                        break;
                    }

                    if (transform.childCount > 0)
                    {
                        child = transform.gameObject.FindChild(name, dive);
                        if (child != null)
                            break;
                    }
                }
            }

        return child;
    }
    public static string GetFullName(this GameObject obj)
    {
        string str = obj.name;
        for (Transform parent = obj.transform.parent;
             parent != null;
             parent = parent.parent)
        {
            str = parent.gameObject.name + "/" + str;
        }

        return str;
    }
    public static bool TryGetResourceLocator(this Il2CppSystem.Object key, out IResourceLocator result)
    {
        if (key != null)
        {
            foreach (IResourceLocator resourceLocator in new Il2CppSystem.Collections.Generic.List<IResourceLocator>(Addressables.ResourceLocators))
            {
                var first = resourceLocator.Keys.ToArray().FirstOrDefault(x => x.Equals(key));
                if (first == null) continue;
                result = resourceLocator;
                return true;
            }
        }
        result = null!;
        return false;
    }

    public static Dictionary<TK, TV> ToMonoDictionary<TK, TV>(this Il2CppSystem.Collections.Generic.Dictionary<TK, TV> @this)
    {
        Dictionary<TK, TV> dictionary = new Dictionary<TK, TV>();
        foreach (var keyValuePair in @this)
        {
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return dictionary;
    }
    public static Il2CppSystem.Collections.Generic.Dictionary<TK, TV> ToIL2CPPDictionary<TK, TV>(this Dictionary<TK, TV> @this)
    {

        Il2CppSystem.Collections.Generic.Dictionary<TK, TV> dictionary =
            new Il2CppSystem.Collections.Generic.Dictionary<TK, TV>();
        foreach (var keyValuePair in @this)
        {
            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }

        return dictionary;
            
    }
    public static List<T> ToMonoList<T>(this Il2CppSystem.Collections.Generic.List<T> @this)
    {
        List<T> list = new List<T>();
        foreach (var t in @this)
        {
            list.Add(t);
        }

        return list;
    }
    public static Il2CppSystem.Collections.Generic.List<T> ToIL2CPPList<T>(this System.Collections.Generic.List<T> @this)
    {
        //UnityEngine.GUI.BeginScrollView
        Il2CppSystem.Collections.Generic.List<T> list = new Il2CppSystem.Collections.Generic.List<T>();
        foreach (var t in @this)
        {
            list.Add(t);
        }

        return list;
    }

    public static SRLESaveSystem.Vector3Save ToVector3Save(this Vector3 @this) => new SRLESaveSystem.Vector3Save()
    {
        x = @this.x,
        z = @this.z,
        y = @this.y
    };
    public static Vector3 ToVector3Save(this SRLESaveSystem.Vector3Save @this) => new Vector3()
    {
        x = @this.x,
        z = @this.z,
        y = @this.y
    };

}
