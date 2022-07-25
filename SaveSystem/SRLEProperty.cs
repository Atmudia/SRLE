using System.IO;
using MonomiPark.SlimeRancher.Persist;

namespace SRLE.SaveSystem
{
    public class SRLEProperty : PersistedDataSet
    {
        public override string Identifier => "SRLEProperty";
        public override uint Version => 1U;
        public override void LoadData(BinaryReader reader)
        {
            property = reader.ReadString();
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(property);
        }

        public string property = "";
    }
}