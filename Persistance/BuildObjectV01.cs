using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SRLE.Persistance
{
    [RegisterInIL2CPP]
    public class BuildObjectV01 : PersistedDataSet
    {
        public Vector3V01 pos;
        public Vector3V01 euler;
        public Vector3V01 scale;

        public override string Identifier { get; } = "BBBO";

        public override uint Version => 1;

        public BuildObjectV01() { }

        public override void LoadData(Il2CppSystem.IO.BinaryReader reader)
        {
            
            pos = LoadPersistable<Vector3V01>(reader);
            euler = LoadPersistable<Vector3V01>(reader);
            scale = LoadPersistable<Vector3V01>(reader);
        }

        public override void WriteData(Il2CppSystem.IO.BinaryWriter writer)
        {
            WritePersistable(writer, pos);
            WritePersistable(writer, euler);
            WritePersistable(writer, scale);
        }
    }
}