using System.IO;
using MonomiPark.SlimeRancher.Persist;
using SRML.Console;
using SRML.SR.SaveSystem.Data.Partial;
using UnityEngine;

namespace SRLE.SaveSystem
{
    public class SRLESave : PersistedDataSet
    {
        public override string Identifier => "SRLESave";
        public override uint Version => 4U;
        


        public override void LoadData(BinaryReader reader)
        {
            position = PersistedDataSet.LoadPersistable<Vector3V02>(reader);
            rotation = PersistedDataSet.LoadPersistable<Vector3V02>(reader);
            scale = PersistedDataSet.LoadPersistable<Vector3V02>(reader);

        }

        public override void WriteData(BinaryWriter writer)
        {
            WritePersistable(writer, position);
            WritePersistable(writer, rotation);
            WritePersistable(writer, scale);

        }

        public Vector3V02 position;
        public Vector3V02 rotation;
        public Vector3V02 scale;
        public string modid = "none";
    }
}