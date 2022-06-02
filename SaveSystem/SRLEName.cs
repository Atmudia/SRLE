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
        }

        public override void WriteData(BinaryWriter writer)
        {
            writer.Write(nameOfLevel);
        }

        public string nameOfLevel;
    }
}