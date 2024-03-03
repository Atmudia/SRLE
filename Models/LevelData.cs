using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SRLE.Models;

public class LevelData
{
    public string LevelName;
    public Dictionary<string, string> Dependencies;
    public Dictionary<uint, List<BuildObjectData>> BuildObjects;
    [JsonIgnore] public string Path;
}
public class BuildObjectData
{
    public Vector3Save Pos;
    public Vector3Save Rot;
    public Vector3Save Scale;
    public uint BuildID;
    public uint HandlerID;
    public string SceneGroup;

    public Dictionary<string, string> Properties;
    public class Vector3Save
    {
        public float x;
        public float y;
        public float z;

        public static Vector3 RevertToVector3(Vector3Save vector3Save)
        {
            return new Vector3(vector3Save.x, vector3Save.y, vector3Save.z);
        }
        public static Vector3Save ToVector3Save(Vector3 vector3)
        {
            return new Vector3Save()
            {
                x = vector3.x,
                y = vector3.y,
                z = vector3.z
            };
        }
    }
     
}