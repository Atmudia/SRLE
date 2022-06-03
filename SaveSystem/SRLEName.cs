using System.Collections.Generic;
using System.IO;
using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Serializable.Optional;
using SRML.Console;

namespace SRLE.SaveSystem
{
    public class SRLEName : PersistedDataSet
    {
        public override string Identifier => "SRLEName";
        public override uint Version => 0U;


        public override void LoadData(BinaryReader reader)
        {
            nameOfLevel = reader.ReadString();
            objects = base.LoadDictionary<SRLEId, List<SRLESave>>(reader, (BinaryReader r) => PersistedDataSet.LoadPersistable<SRLEId>(r), PersistedDataSet.LoadList<SRLESave>);
            Console.Log(objects.Count.ToString());
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(nameOfLevel);
            base.WriteDictionary<SRLEId, List<SRLESave>>(writer, this.objects, WritePersistable, PersistedDataSet.WriteList<SRLESave>);        }
        

        public string nameOfLevel;
        public Dictionary<SRLEId, List<SRLESave>> objects;
    }
}