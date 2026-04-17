using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SRLE.Models
{
    [Serializable]
    public class IdClass
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        public string Path{ get; set; }
        public string Zone { get; set; }
        public int HashCode { get; set; }
        [JsonIgnore] public GameObject gameObject;
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
}