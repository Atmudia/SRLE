using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SRLE;


public class WorldV01
{
    public string WorldName;
    public Dictionary<string, string> Dependencies;
    public Dictionary<uint, List<BuildObject>> BuildObjects;

    [JsonIgnore] public string Path;


}
public class BuildObject
{
    public Vector3Save Pos;
    public Vector3Save Rot;
    public Vector3Save Scale;

    public Dictionary<string, string> Properties;
    public class Vector3Save
    {
        public float x;
        public float y;
        public float z;
    }
}