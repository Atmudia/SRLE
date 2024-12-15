using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace SRLE.Models;

[Serializable]
public class IdClass
{
    public string Name { get; set; }
    public uint Id { get; set; }
    public string Path{ get; set; }
    public string Scene { get; set; }
    public int HashCode { get; set; }
    [JsonIgnore] public GameObject GameObject;
}
public class ModdedIdClass : IdClass
{
    public string ModId { get; set; }
}
public class IdCategoryData
{
    public string CategoryName { get; set; }
    public List<IdClass> Objects { get; set; }
}