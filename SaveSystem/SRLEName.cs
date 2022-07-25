using System.Collections.Generic;
using System.IO;
using MonomiPark.SlimeRancher.Persist;

namespace SRLE.SaveSystem
{

    public enum WorldType
    {
        SEA, 
        DESERT,
        VOID,
        STANDARD,
    }
    public class SRLEName : PersistedDataSet
    {
        public override string Identifier => "SRLEName";
        public override uint Version => 0U;

        public static SRLEName Create(string nameOFLevel, WorldType worldType) => new()
        {
            nameOfLevel = nameOFLevel,
            worldType = worldType,
        };

        public override void LoadData(BinaryReader reader)
        {
            nameOfLevel = reader.ReadString();
            worldType = (WorldType) reader.ReadInt32();
            spriteType = reader.ReadInt32();

            isUsingModdedObjects = reader.ReadBoolean();
            
            this.objects = base.LoadDictionary(reader, r => r.ReadString(), LoadList<SRLESave>);        
        }

        public override void WriteData(BinaryWriter writer)
        {
            
            writer.Write(nameOfLevel);
            writer.Write((int) worldType);
            writer.Write(spriteType);
            writer.Write(isUsingModdedObjects);

            base.WriteDictionary(writer, this.objects, delegate(BinaryWriter w, string k)
            {
                w.Write(k);
            }, WriteList);
        }
        

        public string nameOfLevel;
        public bool isUsingModdedObjects = false;
        public WorldType worldType = WorldType.STANDARD;
        public int spriteType = 0;
        public string nameOfFile;

        public Dictionary<string, List<SRLESave>> objects = new Dictionary<string, List<SRLESave>>();
    }
}