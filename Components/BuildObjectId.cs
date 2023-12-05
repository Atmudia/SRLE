using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace SRLE.Components;

public class BuildObjectId : MonoBehaviour
{
    public BuildObjectId(IntPtr value) : base(value) { }
    public BuildObjects.IdClass IdClass;
    public static List<BuildObjectId> ListedObjects = new List<BuildObjectId>();
    public BuildObject buildObject;

    public void Awake()
    {
        if (buildObject == null)
        {
            buildObject = new BuildObject
            {
                Properties = new Dictionary<string, string>()
            };
        }
        ListedObjects.Add(this);
        MelonLogger.Msg("hello");
    }

    public void OnDestroy()
    {
        ListedObjects.Remove(this);
    }
}