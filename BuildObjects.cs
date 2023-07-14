using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SRLE;

public static class BuildObjects
{
    public class IdClass
    {
        public string Name;
        public uint Id;
        public string Path;
        public string Scene;
        public int HashCode;

        [JsonIgnore] public GameObject GameObject;
        public override bool Equals(object obj)
        {
            if (obj is not IdClass idClass) return false;
            return idClass.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.HashCode;
        }
    }

    public class ModdedIdClass : IdClass
    {
        public string modid;
    }

    public class Category
    {
        public string CategoryName;
        public List<IdClass> Objects = new List<IdClass>();
    }
}
