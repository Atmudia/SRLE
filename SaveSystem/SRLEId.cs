using System.IO;
using MonomiPark.SlimeRancher.Persist;

namespace SRLE.SaveSystem
{
    public class SRLEId : PersistedDataSet
    {
        public override string Identifier => "SRLEId";
        public override uint Version => 5U;
        public ulong id;


        public override void LoadData(BinaryReader reader)
        {
            id = reader.ReadUInt64();
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(id);
        }
    }
}