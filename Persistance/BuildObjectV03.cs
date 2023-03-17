using Il2CppMonomiPark.SlimeRancher.Persist;

namespace SRLE.Persistance
{
    [RegisterInIL2CPP]
    public class BuildObjectV03 : VersionedPersistedDataSet<BuildObjectV02>
    {
        public Vector3V01 pos;
        public Vector3V01 euler;
        public Vector3V01 scale;
        public uint BuildID;
        public uint HandlerID;

        public override string Identifier => "BBBO";

        public override uint Version => 3;

        public BuildObjectV03() { }

        public override void LoadData(Il2CppSystem.IO.BinaryReader reader)
        {
            pos = LoadPersistable<Vector3V01>(reader);
            euler = LoadPersistable<Vector3V01>(reader);
            scale = LoadPersistable<Vector3V01>(reader);
            BuildID = reader.ReadUInt32();
            HandlerID = reader.ReadUInt32();

            
            //TODO Complete this
            /*
            if (BuildID > World.LastBuildID)
            {
                World.LastBuildID = BuildID;
            }
            if (HandlerID > Globals.LastHandlerID)
            {
                Globals.LastHandlerID = HandlerID;
            }
            */
        }

        public override void UpgradeFrom(BuildObjectV02 legacyData)
        {
            pos = legacyData.pos;
            euler = legacyData.euler;
            scale = legacyData.scale;
            //BuildID = World.LastBuildID++;
            HandlerID = Globals.LastHandlerID++;
        }

        public override void WriteData(Il2CppSystem.IO.BinaryWriter writer)
        {
            WritePersistable(writer, pos);
            WritePersistable(writer, euler);
            WritePersistable(writer, scale);
            writer.Write(BuildID);
            writer.Write(HandlerID);
        }
    }
}
