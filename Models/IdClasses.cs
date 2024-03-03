using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SRLE.Models;

[Serializable]
public class IdClass
{
    public string Name;
    public uint Id;
    public string Path;
    public string Scene;
    public int HashCode;
    [JsonIgnore] public GameObject GameObject;
}
public class ModdedIdClass : IdClass
{
    public string ModId;
}
public class IdCategoryData
{
    public string CategoryName;
    public List<IdClass> Objects;
}