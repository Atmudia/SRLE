using System;
using System.Collections.Generic;
using UnityEngine;

namespace SRLE.Components;

public class BuildObjectId : MonoBehaviour
{
    public BuildObjectId(IntPtr value) : base(value) { }
    public BuildObjects.IdClass IdClass;

    public static List<BuildObjectId> ListedObjects = new List<BuildObjectId>();

    public void Awake()
    {
        ListedObjects.Add(this);
    }

    public void OnDestroy()
    {
        ListedObjects.Remove(this);
    }
}