using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SRLE.Persistance
{
    [RegisterInIL2CPP]
    public class BuildObjectV02 : VersionedPersistedDataSet<BuildObjectV01>
    {
        public Vector3V01 pos;
        public Vector3V01 euler;
        public Vector3V01 scale;

        public override string Identifier => "BBBO";

        public override uint Version => 2;

        public BuildObjectV02() { }

        public override void LoadData(Il2CppSystem.IO.BinaryReader reader)
        {
            pos = LoadPersistable<Vector3V01>(reader);
            euler = LoadPersistable<Vector3V01>(reader);
            scale = LoadPersistable<Vector3V01>(reader);
        }

        public override void UpgradeFrom(BuildObjectV01 legacyData)
        {
            pos = legacyData.pos;
            euler = legacyData.euler;
            scale = legacyData.scale;
        }

        public override void WriteData(Il2CppSystem.IO.BinaryWriter writer)
        {
            WritePersistable(writer, pos);
            WritePersistable(writer, euler);
            WritePersistable(writer, scale);
        }
    }
}
