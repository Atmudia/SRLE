using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

namespace SRLE.Models;

public class LevelData
{
    public string LevelName { get; set; }
    public Dictionary<string, string> Dependencies { get; set; }
    public Dictionary<uint, List<BuildObjectData>> BuildObjects { get; set; }
    [JsonIgnore] public string Path;
}
public class BuildObjectData
{
    public Vector3Save Pos { get; set; }
    public Vector3Save Rot { get; set; }
    public Vector3Save Scale { get; set; }
    public uint BuildID{ get; set; }
    public uint HandlerID { get; set; }
    public string SceneGroup{ get; set; }

    public Dictionary<string, string> Properties { get; set; }
    public class Vector3Save
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

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