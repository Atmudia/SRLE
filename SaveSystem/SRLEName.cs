using System.Collections.Generic;
using System.IO;
using MonomiPark.SlimeRancher.Persist;

namespace SRLE.SaveSystem
{
    public class SRLEName : PersistedDataSet
    {
        public override string Identifier => "SRLEName";
        public override uint Version => 0U;


        public override void LoadData(BinaryReader reader)
        {
            nameOfLevel = reader.ReadString();
            isUsingModdedObjects = reader.ReadBoolean();
            this.objects = base.LoadDictionary<ulong, List<SRLESave>>(reader, (BinaryReader r) => r.ReadUInt64(), (BinaryReader r) => PersistedDataSet.LoadList<SRLESave>(r));        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(nameOfLevel);
            writer.Write(isUsingModdedObjects);

            base.WriteDictionary<ulong, List<SRLESave>>(writer, this.objects, delegate(BinaryWriter w, ulong k)
            {
                w.Write(k);
            }, PersistedDataSet.WriteList<SRLESave>);
        }
        

        public string nameOfLevel;
        public bool isUsingModdedObjects = false;
        public Dictionary<ulong, List<SRLESave>> objects = new Dictionary<ulong, List<SRLESave>>();
    }
}