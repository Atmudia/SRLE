using System.Collections.Generic;
using System.IO;
using MonomiPark.SlimeRancher.Persist;

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
            dictionaryWithProperties = base.LoadDictionary(reader, r => r.ReadString(), SRLEProperty.LoadPersistable<SRLEProperty>);      
            
            
            

        }

        public override void WriteData(BinaryWriter writer)
        {
            WritePersistable(writer, position);
            WritePersistable(writer, rotation);
            WritePersistable(writer, scale);
            base.WriteDictionary(writer, this.dictionaryWithProperties, delegate(BinaryWriter w, string k)
            {
                w.Write(k);
            }, WritePersistable);            
        }

        public Vector3V02 position;
        public Vector3V02 rotation;
        public Vector3V02 scale;
        public Dictionary<string, SRLEProperty> dictionaryWithProperties = new Dictionary<string, SRLEProperty>();
        public string modid = "none";
    }
}