using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SRLE.Persistance
{
    [RegisterInIL2CPP]
    public class StringV01 : PersistedDataSet
    {
        public string value;

        public override string Identifier => "BBS1";

        public override uint Version => 1;

        public override void LoadData(Il2CppSystem.IO.BinaryReader reader)
        {
            value = reader.ReadString();
        }

        public override void WriteData(Il2CppSystem.IO.BinaryWriter writer)
        {
            writer.Write(value);
        }
    }
}
