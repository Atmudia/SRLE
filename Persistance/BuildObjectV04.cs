using System.Collections.Generic;
using Il2CppMonomiPark.SlimeRancher.Persist;
using Il2CppSystem.IO;
using MelonLoader;
using SRLE.Collections;

namespace SRLE.Persistance
{ 
    [RegisterInIL2CPP]
    public class BuildObjectV04 : VersionedPersistedDataSet<BuildObjectV03>
    {
        public Vector3V01 pos;
        public Vector3V01 euler;
        public Vector3V01 scale;
        public uint BuildID;
        public uint HandlerID;

        public Dictionary<string, StringV01> Data;

        public override string Identifier
        {
            get
            {
                return "BBBO";
            }
        }

        public override uint Version
        {
            get
            {
                return 4;
            }
        }

        public BuildObjectV04() { }

        public override void LoadData(Il2CppSystem.IO.BinaryReader reader)
        {
            pos = LoadPersistable<Vector3V01>(reader);
            euler = LoadPersistable<Vector3V01>(reader);
            scale = LoadPersistable<Vector3V01>(reader);
            BuildID = reader.ReadUInt32();
            HandlerID = reader.ReadUInt32();
            Data = LoadDictionary<string, StringV01>(reader, new System.Func<BinaryReader, string>(binaryReader => binaryReader.ReadString()), new System.Func<BinaryReader, StringV01>(LoadPersistable<StringV01>)).ToMonoDictionary();
            //TODO Complete this
            /*if (BuildID > World.LastBuildID)
            {
                World.LastBuildID = BuildID;
            }
            if (HandlerID > Globals.LastHandlerID)
            {
                Globals.LastHandlerID = HandlerID;
            }
            */
        }

        public override void UpgradeFrom(BuildObjectV03 legacyData)
        {
            pos = legacyData.pos;
            euler = legacyData.euler;
            scale = legacyData.scale;
            BuildID = legacyData.BuildID;
            HandlerID = legacyData.HandlerID;
            Data = new Dictionary<string, StringV01>();
        }

        public override void WriteData(Il2CppSystem.IO.BinaryWriter writer)
        {
            WritePersistable(writer, pos);
            WritePersistable(writer, euler);
            WritePersistable(writer, scale);
            writer.Write(BuildID);
            writer.Write(HandlerID);
            
            
             
            
            WriteDictionary(writer, Data.WrapToIl2Cpp().Cast<Il2CppSystem.Collections.Generic.Dictionary<string, StringV01>>(), new System.Action<Il2CppSystem.IO.BinaryWriter, string>(
                (binaryWriter, s) =>
                {
                    binaryWriter.Write(s);
                }), new System.Action<Il2CppSystem.IO.BinaryWriter, StringV01>(WritePersistable<StringV01>));
                
        }
    }
}
